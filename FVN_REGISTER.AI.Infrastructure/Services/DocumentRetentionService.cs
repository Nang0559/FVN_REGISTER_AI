using FVN_REGISTER.AI.Application.Abstractions;
using FVN_REGISTER.AI.Application.Contracts;
using FVN_REGISTER.AI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace FVN_REGISTER.AI.Infrastructure.Services;
public sealed class DocumentRetentionService(AiPlatformDbContext db,IOriginalDocumentStore store):IDocumentRetentionService
{
 private const int BatchSize=100;
 public async Task<DocumentRetentionResult> ArchiveExpiredAsync(DateTimeOffset nowUtc,CancellationToken cancellationToken=default)
 {
  var documents=await db.Documents.Where(x=>x.RetentionUntilUtc.HasValue&&x.RetentionUntilUtc<=nowUtc&&x.Status!="Archived"&&x.Status!="Rejected").OrderBy(x=>x.RetentionUntilUtc).Take(BatchSize).ToListAsync(cancellationToken);
  var archived=0;var purged=0;
  foreach(var document in documents)
  {
   cancellationToken.ThrowIfCancellationRequested();var storageKey=document.StorageKey;
   try{if(!storageKey.StartsWith("retained://",StringComparison.OrdinalIgnoreCase)){await store.DeleteAsync(storageKey,cancellationToken);purged++;}}
   catch(FileNotFoundException){ }
   catch(DirectoryNotFoundException){ }
   document.Status="Archived";document.StorageKey=$"retained://purged/{document.Id:N}";document.ContentPurgedAtUtc=nowUtc;archived++;
  }
  if(archived>0)await db.SaveChangesAsync(cancellationToken);return new(archived,purged,nowUtc);
 }
}
