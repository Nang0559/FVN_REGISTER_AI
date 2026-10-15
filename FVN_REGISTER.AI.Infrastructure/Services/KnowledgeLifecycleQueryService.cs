using FVN_REGISTER.AI.Application.Abstractions;
using FVN_REGISTER.AI.Application.Contracts;
using FVN_REGISTER.AI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FVN_REGISTER.AI.Infrastructure.Services;

public sealed class KnowledgeLifecycleQueryService(AiPlatformDbContext db) : IKnowledgeLifecycleQueryService
{
    public async Task<KnowledgePublicationStatus?> GetStatusAsync(string tenantCode, string actorId, Guid documentId,
        IReadOnlyCollection<string> allowedScopes, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tenantCode) || string.IsNullOrWhiteSpace(actorId)) throw new ArgumentException("Tenant and actor are required.");
        if (allowedScopes.Count == 0) return null;
        var documentExists = await db.Documents.AsNoTracking().AnyAsync(x => x.Id == documentId && x.TenantCode == tenantCode.Trim(), cancellationToken);
        if (!documentExists) return null;
        var active = await db.KnowledgeChunks.AsNoTracking().CountAsync(x => x.TenantCode == tenantCode.Trim() && x.DocumentId == documentId && x.IsActive, cancellationToken);
        if (active == 0) return new KnowledgePublicationStatus(documentId, "Unpublished", 0, 0, null, DateTimeOffset.UtcNow);
        var latest = await db.KnowledgeChunks.AsNoTracking().Where(x => x.TenantCode == tenantCode.Trim() && x.DocumentId == documentId && x.IsActive).MaxAsync(x => x.CreatedAtUtc, cancellationToken);
        return new KnowledgePublicationStatus(documentId, "Published", active, 1, latest, null);
    }
}
