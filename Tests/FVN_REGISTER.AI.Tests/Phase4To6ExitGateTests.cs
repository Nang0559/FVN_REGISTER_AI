using System.Text.Json;
using FVN_REGISTER.AI.Infrastructure.Persistence;
using FVN_REGISTER.AI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FVN_REGISTER.AI.Tests;

public sealed class Phase4To6ExitGateTests
{
    [Fact]
    public async Task GoldenDatasetSummary_UsesLatestEvaluationPerCase()
    {
        var options = new DbContextOptionsBuilder<AiPlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new AiPlatformDbContext(options);
        var service = new GoldenDatasetService(db);
        var caseId = await service.AddCaseAsync("T1", "hr01", "HR_CASES", "Attendance", "{\"q\":1}", "{\"a\":1}", null);
        await service.EvaluateCaseAsync("T1", caseId, null, "{\"a\":0}", false, "wrong");
        await service.EvaluateCaseAsync("T1", caseId, null, "{\"a\":1}", true, null);
        var summary = await service.GetSummaryAsync("T1", "HR_CASES");
        Assert.Equal(1, summary.ActiveCases);
        Assert.Equal(1, summary.CompletedCases);
        Assert.Equal(1, summary.PassedCases);
        Assert.Equal(1d, summary.PassRate);
    }

    [Fact]
    public async Task GoldenDatasetRejectsInvalidJson()
    {
        var options = new DbContextOptionsBuilder<AiPlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new AiPlatformDbContext(options);
        var service = new GoldenDatasetService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddCaseAsync("T1", "hr01", "HR_CASES", "Attendance", "not-json", "{}", null));
    }
}
