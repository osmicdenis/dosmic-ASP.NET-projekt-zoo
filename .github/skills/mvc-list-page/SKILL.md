---
name: mvc-list-page
description: Create a new MVC list page with controller action, ViewModel, and Razor view. Use this skill when you need to display a searchable or sortable list of items (animals, staff, feedings, etc.) in the Zoo Management application.
argument-hint: "[entity name] [columns to display]"
---

# Create MVC List Page

Use this skill to add new list/index pages to the Zoo Management application. A list page typically displays a collection of items in a table format with optional filtering.

## Architecture Overview

A list page requires three components:

```
Controller.cs (action method)
    ↓
ViewModel (data transfer object)
    ↓
View (Razor .cshtml)
```

## Step 1: Create a ViewModel

### Location
`ASP.NET-projekt/ViewModels/YourEntityListItemViewModel.cs`

### Template
```csharp
namespace ASP.NET_projekt.ViewModels
{
    public class YourEntityListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Add other display properties needed for the list
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
```

**Naming convention:** `{EntityName}ListItemViewModel.cs`

## Step 2: Add Controller Action

### Location
`ASP.NET-projekt/Controllers/YourEntityController.cs`

### Template
```csharp
public IActionResult Index()
{
    // Get data from repository
    var items = this._repository.GetAll(); // or GetAllYourEntities()

    // Map to ViewModel
    var viewModel = items.Select(item => new YourEntityListItemViewModel
    {
        Id = item.Id,
        Name = item.Name,
        Description = item.Description,
        CreatedDate = item.CreatedDate
    }).ToList();

    return View(viewModel);
}
```

### Optional: Add Routing
In the controller class, you can customize routing:
```csharp
[Route("your-custom-url")]
public IActionResult Index()
{
    // ...
}
```

## Step 3: Create the View

### Location
`ASP.NET-projekt/Views/YourEntity/Index.cshtml`

**Directory note:** The folder name must match the controller name (without "Controller")

### Template
```html
@model List<YourEntityListItemViewModel>

@{
    ViewData["Title"] = "Your Entity List";
}

<div class="container mt-5">
    <div class="row mb-3">
        <div class="col-md-8">
            <h2>@ViewData["Title"]</h2>
        </div>
        <div class="col-md-4 text-end">
            <a href="@Url.Action("Create")" class="btn btn-primary">Add New</a>
        </div>
    </div>

    @if (Model.Count == 0)
    {
        <div class="alert alert-info">No items found.</div>
    }
    else
    {
        <table class="table table-striped table-hover">
            <thead class="table-dark">
                <tr>
                    <th>ID</th>
                    <th>Name</th>
                    <th>Description</th>
                    <th>Created</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var item in Model)
                {
                    <tr>
                        <td>@item.Id</td>
                        <td>@item.Name</td>
                        <td>@item.Description</td>
                        <td>@item.CreatedDate.ToShortDateString()</td>
                        <td>
                            <a href="@Url.Action("Details", new { id = item.Id })" class="btn btn-sm btn-info">View</a>
                            <a href="@Url.Action("Edit", new { id = item.Id })" class="btn btn-sm btn-warning">Edit</a>
                            <a href="@Url.Action("Delete", new { id = item.Id })" class="btn btn-sm btn-danger">Delete</a>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

## Step 4: Update Repository Interface (if needed)

If your repository doesn't have a `GetAll()` method, add it to `ASP.NET-projekt/Repositories/IZooRepository.cs`:

```csharp
List<YourEntity> GetAll();
```

And implement in `EfZooRepository.cs`:

```csharp
public List<YourEntity> GetAll()
{
    return this._context.YourEntities.ToList();
}
```

## Testing the List Page

1. Ensure the controller is named `YourEntityController`
2. Ensure the Views folder is `Views/YourEntity/`
3. Ensure the view file is named `Index.cshtml`
4. Navigate to: `http://localhost:XXXX/YourEntity` or `http://localhost:XXXX/your-custom-url` (if custom routing used)

## Common Patterns in This Project

### For Animals
- Controller: `AnimalsController`
- ViewModel: `AnimalListItemViewModel`
- View: `Views/Animals/Index.cshtml`
- URL: `/Animals` or `/zwierzęta`

### For Staff
- Controller: `StaffController`
- ViewModel: `StaffListItemViewModel`
- View: `Views/Staff/Index.cshtml`
- URL: `/Staff` or `/pracownicy`

## Checklist

- [ ] ViewModel created with relevant properties
- [ ] Controller action retrieves and maps data
- [ ] View file created in correct folder
- [ ] Bootstrap classes applied for styling
- [ ] Navigation links (Details, Edit, Delete) functional
- [ ] "Add New" button links to Create action
- [ ] Empty state message displays when no items
- [ ] Table headers are clear and descriptive

## Related Skills
- Use `mvc-edit-form` to create corresponding Create/Edit pages
- Use `entity-framework-model` to manage underlying data models
