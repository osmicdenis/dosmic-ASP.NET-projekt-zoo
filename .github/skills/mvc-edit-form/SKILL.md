---
name: mvc-edit-form
description: Create or modify Edit/Create forms for MVC views, including controller actions, ViewModels with validation, and Razor forms. Use this skill when you need to add data entry pages with form validation and model binding.
argument-hint: "[entity name] [fields and validation rules]"
---

# Create MVC Edit/Create Form

Use this skill to add or modify data entry forms in the Zoo Management application. This skill covers both Create (new item) and Edit (modify existing) scenarios.

## Architecture Overview

An edit/create form requires:

```
ViewModel (with validation attributes)
    ↓
Controller.cs (GET for form display, POST for submission)
    ↓
View (Razor .cshtml with form)
```

## Step 1: Create/Update ViewModel with Validation

### Location
`ASP.NET-projekt/ViewModels/YourEntityDetailsViewModel.cs`

### Template with Validation
```csharp
using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.ViewModels
{
    public class YourEntityDetailsViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 3, 
            ErrorMessage = "Name must be between 3 and 100 characters")]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid number")]
        public int Quantity { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }

        // Foreign key for dropdown selection
        public int CategoryId { get; set; }
        public List<CategoryViewModel> AvailableCategories { get; set; }
    }
}
```

### Common Validation Attributes
- `[Required]` - Field must have a value
- `[StringLength(max)]` - Maximum character length
- `[Range(min, max)]` - Numeric range
- `[EmailAddress]` - Valid email format
- `[DataType(DataType.Date)]` - Date picker
- `[Compare("OtherField")]` - Must match another field (e.g., passwords)

## Step 2: Add Controller Actions

### Location
`ASP.NET-projekt/Controllers/YourEntityController.cs`

### GET Action (Display Empty Form or for Editing)
```csharp
[HttpGet]
public IActionResult Create()
{
    var viewModel = new YourEntityDetailsViewModel
    {
        AvailableCategories = this._repository.GetCategories()
            .Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name })
            .ToList()
    };
    return View(viewModel);
}

[HttpGet]
public IActionResult Edit(int? id)
{
    if (id == null)
        return NotFound();

    var item = this._repository.GetById(id.Value);
    if (item == null)
        return NotFound();

    var viewModel = new YourEntityDetailsViewModel
    {
        Id = item.Id,
        Name = item.Name,
        Description = item.Description,
        AvailableCategories = this._repository.GetCategories()
            .Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name })
            .ToList()
    };
    return View(viewModel);
}
```

### POST Action (Handle Form Submission)
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(YourEntityDetailsViewModel viewModel)
{
    if (!ModelState.IsValid)
    {
        // Reload dropdown data if validation fails
        viewModel.AvailableCategories = this._repository.GetCategories()
            .Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name })
            .ToList();
        return View(viewModel);
    }

    var entity = new YourEntity
    {
        Name = viewModel.Name,
        Description = viewModel.Description
    };

    this._repository.Add(entity);
    this._repository.SaveChanges();

    return RedirectToAction("Details", new { id = entity.Id });
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Edit(int id, YourEntityDetailsViewModel viewModel)
{
    if (id != viewModel.Id)
        return BadRequest();

    if (!ModelState.IsValid)
    {
        viewModel.AvailableCategories = this._repository.GetCategories()
            .Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name })
            .ToList();
        return View(viewModel);
    }

    var entity = this._repository.GetById(id);
    if (entity == null)
        return NotFound();

    entity.Name = viewModel.Name;
    entity.Description = viewModel.Description;

    this._repository.Update(entity);
    this._repository.SaveChanges();

    return RedirectToAction("Details", new { id = id });
}
```

## Step 3: Create the View

### Location
`ASP.NET-projekt/Views/YourEntity/Create.cshtml` or `Edit.cshtml`

### Template
```html
@model YourEntityDetailsViewModel

@{
    ViewData["Title"] = Model.Id == 0 ? "Create New" : "Edit";
}

<div class="container mt-5">
    <div class="row">
        <div class="col-md-8 offset-md-2">
            <h2>@ViewData["Title"]</h2>
            <hr />

            <form asp-action="@(Model.Id == 0 ? "Create" : "Edit")" method="post">
                <div asp-validation-summary="ModelOnly" class="text-danger"></div>

                @if (Model.Id != 0)
                {
                    <input type="hidden" asp-for="Id" />
                }

                <div class="form-group mb-3">
                    <label asp-for="Name" class="form-label"></label>
                    <input asp-for="Name" class="form-control" />
                    <span asp-validation-for="Name" class="text-danger"></span>
                </div>

                <div class="form-group mb-3">
                    <label asp-for="Description" class="form-label"></label>
                    <textarea asp-for="Description" class="form-control" rows="4"></textarea>
                    <span asp-validation-for="Description" class="text-danger"></span>
                </div>

                <div class="form-group mb-3">
                    <label asp-for="Quantity" class="form-label"></label>
                    <input asp-for="Quantity" class="form-control" type="number" />
                    <span asp-validation-for="Quantity" class="text-danger"></span>
                </div>

                <!-- Dropdown for Foreign Key -->
                <div class="form-group mb-3">
                    <label asp-for="CategoryId" class="form-label">Category</label>
                    <select asp-for="CategoryId" asp-items="@(new SelectList(Model.AvailableCategories, "Id", "Name"))" class="form-select">
                        <option value="">-- Select Category --</option>
                    </select>
                    <span asp-validation-for="CategoryId" class="text-danger"></span>
                </div>

                <div class="form-group">
                    <button type="submit" class="btn btn-primary">@(Model.Id == 0 ? "Create" : "Update")</button>
                    <a href="@Url.Action("Index")" class="btn btn-secondary">Cancel</a>
                </div>
            </form>
        </div>
    </div>
</div>

@section Scripts {
    @{
        await Html.RenderPartialAsync("_ValidationScriptsPartial");
    }
}
```

## Step 4: Update Repository (if needed)

Add to `ASP.NET-projekt/Repositories/IZooRepository.cs`:

```csharp
void Add(YourEntity entity);
void Update(YourEntity entity);
void SaveChanges();
```

Implement in `EfZooRepository.cs`:

```csharp
public void Add(YourEntity entity)
{
    this._context.YourEntities.Add(entity);
}

public void Update(YourEntity entity)
{
    this._context.YourEntities.Update(entity);
}

public void SaveChanges()
{
    this._context.SaveChanges();
}
```

## Form Patterns in This Project

### Example: Animals Create/Edit
- ViewModel: `AnimalDetailsViewModel`
- Views: `Views/Animals/Create.cshtml`, `Edit.cshtml`
- Controller: `AnimalsController`

### Example: Staff Create/Edit
- ViewModel: `StaffDetailsViewModel`
- Views: `Views/Staff/Create.cshtml`, `Edit.cshtml`
- Controller: `StaffController`

## Important Notes

### Anti-Forgery Token
Always include `[ValidateAntiForgeryToken]` on POST actions and the `@Html.AntiForgeryToken()` in forms:

```csharp
// In controller
[ValidateAntiForgeryToken]
public IActionResult Create(...)

// In view (added automatically with form tag helpers)
<form asp-action="Create" method="post">
```

### Model Binding
- Form field names must match ViewModel property names
- Use `asp-for` to auto-generate correct names
- Use `asp-items` for select lists with foreign keys

### Validation
- Server-side validation is required (`ModelState.IsValid`)
- Client-side validation happens automatically via `_ValidationScriptsPartial`
- Always reload dropdown/select data on validation failure

## Checklist

- [ ] ViewModel created with [Required] and validation attributes
- [ ] Create GET action loads dropdown/related data
- [ ] Create POST action validates and saves
- [ ] Edit GET action loads existing data
- [ ] Edit POST action updates data
- [ ] View has correct form action (Create or Edit)
- [ ] Form inputs use `asp-for` attributes
- [ ] Validation messages display on errors
- [ ] Foreign key dropdowns populate correctly
- [ ] Cancel button works
- [ ] Redirect to Details after successful save

## Related Skills
- Use `mvc-list-page` to display list of items with links to Create/Edit
- Use `entity-framework-model` to manage underlying data models
