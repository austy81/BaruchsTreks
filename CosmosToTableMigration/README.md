# Cosmos DB to Azure Table Storage Migration Tool

This console application migrates trip data from Azure Cosmos DB to Azure Table Storage.

## Prerequisites

- .NET 6.0 or later
- Access to your Azure Cosmos DB account
- Access to an Azure Storage account

## Configuration

Before running the migration tool, update the `appsettings.json` file with your connection strings:

```json
{
  "CosmosDb": {
    "ConnectionString": "YOUR_COSMOS_CONNECTION_STRING",
    "DatabaseName": "YOUR_COSMOS_DB_NAME"
  },
  "AzureStorage": {
    "ConnectionString": "YOUR_STORAGE_ACCOUNT_CONNECTION_STRING"
  }
}
```

## How to Run

1. Update the `appsettings.json` file with your connection strings
2. Build the application: `dotnet build`
3. Run the application: `dotnet run`

## Migration Process

The tool will:
1. Connect to your Cosmos DB and fetch all trip records
2. Create a "Trips" table in Azure Table Storage if it doesn't exist
3. Migrate each trip to the Azure Table Storage
4. Display progress and results in the console

## Partitioning Strategy

By default, all trips are stored in a single partition with the PartitionKey "Trips". 
You may want to modify this strategy based on your specific query patterns and data volume.

## Data Mapping

The tool handles complex types like `LocalGeometry` by serializing them to JSON strings.
Enum values are stored as strings in Azure Table Storage.

## Error Handling

The migration tool includes basic error handling and will report any failures during migration.
It will continue processing other records even if some fail.

## Security Best Practices

### Handling Connection Strings

This migration tool requires connection strings to both Azure Cosmos DB and Azure Storage. These connection strings contain sensitive information and should be handled securely:

1. **Never commit connection strings to source control**
   - The `appsettings.json` file has been added to `.gitignore` to prevent accidental commits
   - Use `appsettings.template.json` as a template and create your own local `appsettings.json` with actual values

2. **Secure your connection strings**
   - In production environments, consider using Azure Key Vault or environment variables
   - Rotate your keys periodically
   - Use managed identities when possible

3. **Before running the migration**
   - Make sure you have the correct connection strings in your local `appsettings.json` file
   - Ensure you have the necessary permissions on both the source and target services
