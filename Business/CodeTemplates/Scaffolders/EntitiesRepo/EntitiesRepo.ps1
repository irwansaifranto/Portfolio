[T4Scaffolding.Scaffolder(Description = "create model repository")][CmdletBinding()]
param(
	[parameter(Position = 0, Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$ModelType,
	[string]$DbContextType,    
    [string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false
)

# Ensure you've referenced System.Data.Entity
(Get-Project $Project).Object.References.Add("System.Data.Entity") | Out-Null

$foundModelType = Get-ProjectType $ModelType -Project $Project
if (!$foundModelType) { return }

$primaryKey = Get-PrimaryKey $foundModelType.FullName -Project $Project -ErrorIfNotFound
if (!$primaryKey) { return }

# Change product_category_data to ProductCategoryData
$modelNamePascalCase = [System.Text.RegularExpressions.Regex]::Replace($foundModelType.Name, "_(\p{Ll})",{
        param($m)
		$m.Groups[1].Value.ToUpper()
	})
$modelNamePascalCase = [System.Text.RegularExpressions.Regex]::Replace($modelNamePascalCase, "\b(\p{Ll})",{
	param($m)
		$m.Groups[0].Value.ToUpper()
	})

if(!$DbContextType) { $DbContextType = [System.Text.RegularExpressions.Regex]::Replace((Get-Project $Project).Name, "[^a-zA-Z0-9]", "") + ".Context" }

$outputPath = Join-Path Abstract ("I" + $modelNamePascalCase + "Repository")

if (!$foundDbContextType) { $foundDbContextType = Get-ProjectType $DbContextType -Project $Project }
if (!$foundDbContextType) { return }

$modelTypePluralized = Get-PluralizedWord $foundModelType.Name
$defaultNamespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value
$abstractRepositoryNamespace = [T4Scaffolding.Namespaces]::Normalize($defaultNamespace + "." + [System.IO.Path]::GetDirectoryName($outputPath).Replace([System.IO.Path]::DirectorySeparatorChar, "."))
$modelTypeNamespace = [T4Scaffolding.Namespaces]::GetNamespace($foundModelType.FullName)

Add-ProjectItemViaTemplate $outputPath -Template IEntitiesRepoTemplate -Model @{
	ModelType = [MarshalByRefObject]$foundModelType;
	ModelNamePascalCase = $modelNamePascalCase;
	PrimaryKey = [string]$primaryKey; 
	DefaultNamespace = $defaultNamespace; 
	RepositoryNamespace = $abstractRepositoryNamespace; 
	ModelTypeNamespace = $modelTypeNamespace; 
	ModelTypePluralized = [string]$modelTypePluralized; 
	DbContextNamespace = $foundDbContextType.Namespace.FullName;
	DbContextType = [MarshalByRefObject]$foundDbContextType;
} -SuccessMessage "Added abstract repository '{0}'" -TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

$outputPath = Join-Path Concrete ("EF" + $modelNamePascalCase + "Repository")
$concreteRepositoryNamespace = [T4Scaffolding.Namespaces]::Normalize($defaultNamespace + "." + [System.IO.Path]::GetDirectoryName($outputPath).Replace([System.IO.Path]::DirectorySeparatorChar, "."))

Add-ProjectItemViaTemplate $outputPath -Template EFEntitiesRepoTemplate -Model @{
	ModelType = [MarshalByRefObject]$foundModelType;
	ModelNamePascalCase = $modelNamePascalCase;
	PrimaryKey = [string]$primaryKey; 
	DefaultNamespace = $defaultNamespace; 
	RepositoryNamespace = $concreteRepositoryNamespace; 
	ModelTypeNamespace = $modelTypeNamespace;
	AbstractRepositoryNamespace = $abstractRepositoryNamespace;
	ModelTypePluralized = [string]$modelTypePluralized; 
	DbContextNamespace = $foundDbContextType.Namespace.FullName;
	DbContextType = [MarshalByRefObject]$foundDbContextType;
} -SuccessMessage "Added concrete repository '{0}'" -TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force