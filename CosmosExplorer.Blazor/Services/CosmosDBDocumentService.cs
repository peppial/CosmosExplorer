﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosExplorer.Core.Command;
using CosmosExplorer.Core.Query;
using CosmosExplorer.Core.Connection;
using CosmosExplorer.Core.Models;

namespace CosmosExplorer.Blazor.Services;

public class CosmosDBDocumentService
{
    private readonly IConnectionService connectionService;
    private readonly IQueryService queryService;
    private readonly ICommandService commandService;
    private readonly StateContainer stateContainer;

    public CosmosDBDocumentService(StateContainer stateContainer, IConnectionService connectionService,
        IQueryService queryService, ICommandService commandService)
    {
        this.connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        this.queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
        this.commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        this.stateContainer = stateContainer ?? throw new ArgumentNullException(nameof(stateContainer));
    }

    public async Task<IEnumerable<IDatabaseModel>> GetDatabasesAsync()
    {
        if (string.IsNullOrEmpty(stateContainer.ConnectionString))
        {
            return new List<IDatabaseModel>();
        }

        return await connectionService.GetDatabasesAsync(stateContainer.ConnectionString, new CancellationToken());
    }

    public async Task<string> ChangeContainerAsync(string databaseName, string containerName)
    {
        stateContainer.Database = databaseName;
        stateContainer.Container = containerName;
        return await connectionService.ChangeContainerAsync(stateContainer.ConnectionString, databaseName,
            containerName);
    }

    public async Task<(IEnumerable<(string, string)>, int)> QueryAsync(string query, int count)
    {
        var result = await queryService.QueryAsync(null, query, count, new CancellationToken());

        return (result.Items.Select(i => (i.Id, i.PartitionKey)), result.Items.Count());
    }

    public async Task<string> GetDocumentAsync(string id, string partitionKey)
    {
        return await queryService.GetDocumentAsync(partitionKey, id);
    }

    public async Task UpdateDocumentAsync(string id, string partitionKey, string documentString)
    {
        await commandService.UpdateDocumentAsync(id, partitionKey, documentString);
    }

    public async Task DeleteDocumentAsync(string id, string partitionKey)
    {
        await commandService.DeleteDocumentAsync(id, partitionKey);
    }
}