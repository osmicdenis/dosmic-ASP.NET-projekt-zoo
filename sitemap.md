# Sitemap

This document maps the available URLs in the application to the controller action that handles them and the view or response that is returned.

## Home

| URL | Controller / Action | View / Result |
| --- | --- | --- |
| `/` | `HomeController.Index` | [Views/Home/Index.cshtml](ASP.NET-projekt/Views/Home/Index.cshtml) |
| `/Home` | `HomeController.Index` | [Views/Home/Index.cshtml](ASP.NET-projekt/Views/Home/Index.cshtml) |
| `/Home/Index` | `HomeController.Index` | [Views/Home/Index.cshtml](ASP.NET-projekt/Views/Home/Index.cshtml) |
| `/Home/Privacy` | `HomeController.Privacy` | [Views/Home/Privacy.cshtml](ASP.NET-projekt/Views/Home/Privacy.cshtml) |
| `/Home/Error` | `HomeController.Error` | [Views/Shared/Error.cshtml](ASP.NET-projekt/Views/Shared/Error.cshtml) |

## Animals

| URL | Controller / Action | View / Result |
| --- | --- | --- |
| `/animals` | `AnimalsController.Index` | [Views/Animals/Index.cshtml](ASP.NET-projekt/Views/Animals/Index.cshtml) |
| `/Animals` | `AnimalsController.Index` | [Views/Animals/Index.cshtml](ASP.NET-projekt/Views/Animals/Index.cshtml) |
| `/Animals/Index` | `AnimalsController.Index` | [Views/Animals/Index.cshtml](ASP.NET-projekt/Views/Animals/Index.cshtml) |
| `/Animals/Create` | `AnimalsController.Create` | [Views/Animals/Create.cshtml](ASP.NET-projekt/Views/Animals/Create.cshtml) |
| `/Animals/Edit/{id}` | `AnimalsController.Edit` | [Views/Animals/Edit.cshtml](ASP.NET-projekt/Views/Animals/Edit.cshtml) |
| `/animals/{id}` | `AnimalsController.Details` | [Views/Animals/Details.cshtml](ASP.NET-projekt/Views/Animals/Details.cshtml) |
| `/Animals/Details/{id}` | `AnimalsController.Details` | [Views/Animals/Details.cshtml](ASP.NET-projekt/Views/Animals/Details.cshtml) |

## Enclosures

| URL | Controller / Action | View / Result |
| --- | --- | --- |
| `/Enclosures` | `EnclosuresController.Index` | [Views/Enclosures/Index.cshtml](ASP.NET-projekt/Views/Enclosures/Index.cshtml) |
| `/Enclosures/Index` | `EnclosuresController.Index` | [Views/Enclosures/Index.cshtml](ASP.NET-projekt/Views/Enclosures/Index.cshtml) |
| `/enclosures/{id}` | `EnclosuresController.Details` | [Views/Enclosures/Details.cshtml](ASP.NET-projekt/Views/Enclosures/Details.cshtml) |
| `/Enclosures/Details/{id}` | `EnclosuresController.Details` | [Views/Enclosures/Details.cshtml](ASP.NET-projekt/Views/Enclosures/Details.cshtml) |

## Feedings

| URL | Controller / Action | View / Result |
| --- | --- | --- |
| `/feeding-schedule` | `FeedingsController.Index` | [Views/Feedings/Index.cshtml](ASP.NET-projekt/Views/Feedings/Index.cshtml) |
| `/Feedings` | `FeedingsController.Index` | [Views/Feedings/Index.cshtml](ASP.NET-projekt/Views/Feedings/Index.cshtml) |
| `/Feedings/Index` | `FeedingsController.Index` | [Views/Feedings/Index.cshtml](ASP.NET-projekt/Views/Feedings/Index.cshtml) |
| `/Feedings/Details/{id}` | `FeedingsController.Details` | [Views/Feedings/Details.cshtml](ASP.NET-projekt/Views/Feedings/Details.cshtml) |

## Staff

| URL | Controller / Action | View / Result |
| --- | --- | --- |
| `/Staff` | `StaffController.Index` | [Views/Staff/Index.cshtml](ASP.NET-projekt/Views/Staff/Index.cshtml) |
| `/Staff/Index` | `StaffController.Index` | [Views/Staff/Index.cshtml](ASP.NET-projekt/Views/Staff/Index.cshtml) |
| `/staff/{role}/{id}` | `StaffController.Details` | [Views/Staff/Details.cshtml](ASP.NET-projekt/Views/Staff/Details.cshtml) |
| `/Staff/Details/{role}/{id}` | `StaffController.Details` | [Views/Staff/Details.cshtml](ASP.NET-projekt/Views/Staff/Details.cshtml) |

## Zoo Map

| URL | Controller / Action | View / Result |
| --- | --- | --- |
| `/zoo-map` | `ZooMapController.Index` | [Views/ZooMap/Index.cshtml](ASP.NET-projekt/Views/ZooMap/Index.cshtml) |
| `/ZooMap` | `ZooMapController.Index` | [Views/ZooMap/Index.cshtml](ASP.NET-projekt/Views/ZooMap/Index.cshtml) |
| `/ZooMap/Index` | `ZooMapController.Index` | [Views/ZooMap/Index.cshtml](ASP.NET-projekt/Views/ZooMap/Index.cshtml) |
| `/ZooMap/Image` | `ZooMapController.Image` | Physical file response for the map image |

## What It Is

This sitemap is a semantic routing model for the MVC app. It shows which URLs exist, which controller action handles each one, and whether the endpoint returns a Razor view or another kind of response.

## What It Does

It gives you a quick way to understand the navigation structure of the application. Instead of searching through controllers and route configuration manually, you can use this file to see the page map, identify where each URL goes, and find the view that powers it.