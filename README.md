### Komponente

1. **DataExchange.Shared** - Zajednički modeli (`RandomNumber`, `CsrngResponse`)
2. **DataExchange.Storage** - In-memory storage implementacija (`ConcurrentDictionary`)
3. **DataExchange.StorageApi** (Port: 7000) - Centralni API za pohranu podataka
4. **DataExchange.WriterApi** (Port: 7001) - Dohvaća podatke iz vanjskog API-ja i šalje ih u Storage
5. **DataExchange.ReaderApi** (Port: 7002) - Čita podatke iz Storage API-ja

### Test projekti

6. **DataExchange.Storage.Tests** - Unit testovi za Storage layer