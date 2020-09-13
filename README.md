
# Spors.Linq.Filter 
![Nuget](https://img.shields.io/nuget/dt/Spors.Linq.Filter?style=flat-square) 
![Website](https://img.shields.io/website?down_color=red&down_message=spors.io&style=flat-square&up_message=spors.io&url=https%3A%2F%2Fgoogle.de)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/LeonSpors/Linq.Filter/.NET%20Core) 
![GitHub last commit](https://img.shields.io/github/last-commit/LeonSpors/Linq.Filter?style=flat-square) 
![codecov](https://codecov.io/gh/LeonSpors/Linq.Filter/branch/master/graph/badge.svg)
![GitHub](https://img.shields.io/github/license/LeonSpors/Linq.Filter?style=flat-square)

## An universal library for filtering list
Spors.Linq.Filter is a .NET library for managing linq filtering of an IEnumerable source. See [documentation](docs/README.md).

## Getting Started

At the moment the only way to get this package is by using nuget.org.

```
Install-Package Spors.Linq.Filter
```

## Usage

```cs
Filter<int> filter = new();

Predicate<int> predicate = val => val > 2;
filter.AddFilter(predicate);

List<int> numbers = new() { 0, 1, 3, 4 };
List<int> resultList = (List<int>) filter.ApplyFilters(numbers);
```