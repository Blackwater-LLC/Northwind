# Northwind

## Introduction

Northwind is an in-house developed library to make CRUD operations extremely easy and type-safe with MongoDB built in .NET 9. It provides a fluent API that allows you to configure your MongoDB connection, define collections, enforce uniqueness, etc, all while suuporting LINQ queries. It was made to abstract the already abstracted complexity of large scale MongoDB interactions to allow you to focus on logic instead of learning in depth MongoDB interactions.

## Features

- **Fluent Configuration:** Easily configure MongoDB groups and repositories using a builder pattern.
- **Type-Safe CRUD Operations:** Strongly typed Create, Read, Update, and Delete operations.
- **Unique Index Enforcement:** Enforce unique constraints on specified properties.
- **LINQ Integration:** Use native LINQ operations (e.g., `.Where`, `.Select`) for flexible queries.
- **Unified Facade:** A centralized service (`INorthwindService<T>`) that exposes all operations in a simple and unified manner.
- **Transaction Support:** Optionally use MongoDB transactions to ensure data integrity.

## Getting Started

### Setting Up the Group

Before using CRUD operations, you need to register your group. For example, if you have an entity called ```TestClass``` that serves as your collection schema:
```csharp
using MongoDB.Driver;
using Northwind.Core.Builders;
using Northwind.Core.Models;
using Northwind.Core.Services;
using Northwind.Api.Models;

var client = new MongoClient(Configuration["MDB_ConnectionString"]);
var database = client.GetDatabase("Northwind");
var collection = database.GetCollection<TestClass>("Tests");

GroupRegistry.Clear();

GroupBuilder<TestClass, int>
    .New(nameof(TestClass), client, database, collection, new NorthwindOptions 
    { 
        UseTransactions = true, 
        ReturnDocumentState = true 
    })
	// Will make the Id property on the TestClass serve as the Primary key (_id)
    .HasPrimaryKey(x => x.Id)
	// Ensures all Name values in the collection are unique and will throw a UNIQUE_INDEX_VIOLATION at violation
    .WithIndex(x => x.Name, unique: true)
	// finalize
    .Build();

var northwind = new NorthwindService<TestClass>();
```

### Using the Unified Facade

Northwind provides a unified facade for CRUD operations and LINQ-enabled queries. Here are some examples:

#### Create
```csharp
var testEntity = new TestClass { Id = 1, Name = "Test" };
var createResult = await northwind.CreateAsync(testEntity);
```

#### Read
You can read using a filter or via LINQ queries.
```csharp
// Using a filter
var readResult = await northwind.ReadAsync(x => x.Id == 1);

// Using LINQ query
var list = await northwind.Query().Where(x => x.Name == "Name").ToListAsync();
var single = await northwind.Query().Where(x => x.Id == 1).FirstOrDefaultAsync();

```


#### Update
You can update using a filter and an update definition, or using the LINQ-enabled update query.
```csharp
// Standard update
var updateDef = Builders<TestClass>.Update.Set(x => x.Name, "NewName");
var updateResult = await northwind.UpdateAsync(x => x.Id == 1, updateDef);

// LINQ-enabled update
var linqUpdateResult = await northwind.UpdateQuery()
    .Where(x => x.Id == 1)
    .Set(x => x.Name, "NewName")
    .ExecuteAsync();

```

#### Delete
You can delete by primary key or use a LINQ-enabled delete query.
```csharp
// Delete by primary key
var deleteResult = await northwind.DeleteAsync(1);

// LINQ-enabled delete
var linqDeleteResult = await northwind.DeleteQuery()
    .Where(x => x.Name == "Test")
    .ExecuteAsync();

```

# API Reference

## INorthwindService<T>

**CreateAsync(T entity):**  
_Asynchronously creates a new entity._  
**Parameters:**  
- `entity` (T): The entity to create.  
**Returns:**  
- `Task<OperationResult<T>>`: The result of the creation operation.

**DeleteAsync<TField>(TField id):**  
_Asynchronously deletes an entity based on its primary key._  
**Parameters:**  
- `id` (TField): The primary key value.  
**Returns:**  
- `Task<OperationResult<T>>`: The result of the deletion operation.

**UpdateAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition):**  
_Asynchronously updates an entity matching the specified filter using the provided update definition._  
**Parameters:**  
- `filter` (Expression<Func<T, bool>>): The filter expression to match the entity.  
- `updateDefinition` (UpdateDefinition<T>): The update definition specifying the changes.  
**Returns:**  
- `Task<OperationResult<T>>`: The result of the update operation.

**ReadAsync(Expression<Func<T, bool>> filter):**  
_Asynchronously reads an entity matching the specified filter._  
**Parameters:**  
- `filter` (Expression<Func<T, bool>>): The filter expression to match the entity.  
**Returns:**  
- `Task<OperationResult<T>>`: The result of the read operation.

**Query():**  
_Returns a LINQ-enabled query for reading entities._  
**Returns:**  
- `INorthwindQuery<T>`: A query interface for LINQ operations.

**UpdateQuery():**  
_Returns a LINQ-enabled update query for updating entities._  
**Returns:**  
- `IUpdateQuery<T>`: An update query interface.

**DeleteQuery():**  
_Returns a LINQ-enabled delete query for deleting entities._  
**Returns:**  
- `IDeleteQuery<T>`: A delete query interface.

## INorthwindQuery<T>

**Where(Expression<Func<T, bool>> predicate):**  
_Applies a filter condition to the query and returns a new query._  
**Parameters:**  
- `predicate` (Expression<Func<T, bool>>): The filter condition.  
**Returns:**  
- `INorthwindQuery<T>`: The filtered query.

**Select(Expression<Func<T, TResult>> selector):**  
_Projects each element of the sequence into a new form._  
**Parameters:**  
- `selector` (Expression<Func<T, TResult>>): The projection function.  
**Returns:**  
- `IQueryable<TResult>`: The projected query.

**ToListAsync():**  
_Asynchronously returns a list of entities._  
**Returns:**  
- `Task<List<T>>`: The list of entities.

**FirstOrDefaultAsync():**  
_Asynchronously returns the first entity matching the query or default._  
**Returns:**  
- `Task<T>`: The first entity or default.

## IUpdateQuery<T>

**Where(Expression<Func<T, bool>> predicate):**  
_Applies a filter condition to the update query._  
**Parameters:**  
- `predicate` (Expression<Func<T, bool>>): The filter condition.  
**Returns:**  
- `IUpdateQuery<T>`: The update query instance.

**Set<TField>(Expression<Func<T, TField>> field, TField value):**  
_Specifies a field to update with a new value._  
**Parameters:**  
- `field` (Expression<Func<T, TField>>): The field selector expression.  
- `value` (TField): The new value.  
**Returns:**  
- `IUpdateQuery<T>`: The update query instance.

**ExecuteAsync():**  
_Executes the update operation._  
**Returns:**  
- `Task<OperationResult<T>>`: The result of the update operation.

## IDeleteQuery<T>

**Where(Expression<Func<T, bool>> predicate):**  
_Applies a filter condition to the delete query._  
**Parameters:**  
- `predicate` (Expression<Func<T, bool>>): The filter condition.  
**Returns:**  
- `IDeleteQuery<T>`: The delete query instance.

**ExecuteAsync():**  
_Executes the delete operation._  
**Returns:**  
- `Task<OperationResult<T>>`: The result of the delete operation.

# Example Controller

This controller can be found in the Northwind.Api solution which serves as the testing Web API for Northwind CRUD operations.

```csharp
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Linq;
using Northwind.Api.Models;
using Northwind.Core.Services;

namespace Northwind.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrudTestingController(INorthwindService<TestClass> northwind) : ControllerBase
    {
        private readonly INorthwindService<TestClass> _northwind = northwind;

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] TestClass testEntity)
        {
            var result = await _northwind.CreateAsync(testEntity);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _northwind.DeleteQuery().Where(x => x.Id == id).ExecuteAsync();
            return Ok(result);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, string newName)
        {
            var result = await _northwind.UpdateQuery()
                .Where(x => x.Id == id)
                .Set(x => x.Name, newName)
                .ExecuteAsync();
            return Ok(result);
        }

        [HttpGet("Read/{id}")]
        public async Task<IActionResult> Read(string id)
        {
            var result = await _northwind.Query().Where(x => x.Name == id).ToListAsync();
            return Ok(result);
        }
    }
}
```
