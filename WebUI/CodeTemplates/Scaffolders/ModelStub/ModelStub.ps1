[T4Scaffolding.Scaffolder(Description = "Enter a description of ModelStub here")][CmdletBinding()]
param(
	[parameter(Position = 0, Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$ModelType,
	[string]$DbContextType,
	[string]$Area,
    [string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false,
	$rootPath
)

$dllPath = Join-Path $TemplateFolders[0] RekadiaPS.dll
Import-Module $dllPath

# Ensure you've referenced System.Data.Entity
(Get-Project $Project).Object.References.Add("System.Data.Entity") | Out-Null

$foundModelType = Get-ProjectType $ModelType -Project Business
if (!$foundModelType) { return }

if ($foundModelType) { $relatedEntities = [Array](Get-RekadiaRelatedEntities $foundModelType.FullName -Project Business) }
if (!$relatedEntities) { $relatedEntities = @() }

$primaryKey = Get-PrimaryKey $foundModelType.FullName -Project Business -ErrorIfNotFound
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

$outputPath = Join-Path Models ($modelNamePascalCase + "/" + $modelNamePascalCase + "PresentationStub")
# We don't create areas here, so just ensure that if you specify one, it already exists
if ($Area) {
	$areaPath = Join-Path Areas $Area
	if (-not (Get-ProjectItem $areaPath -Project $Project)) {
		Write-Error "Cannot find area '$Area'. Make sure it exists already."
		return
	}
	$outputPath = Join-Path $areaPath $outputPath
}

if (!$foundDbContextType) { $foundDbContextType = Get-ProjectType $DbContextType -Project $Project }
if (!$foundDbContextType) { return }

$modelTypePluralized = Get-PluralizedWord $foundModelType.Name
$defaultNamespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value
$ModelStubNamespace = [T4Scaffolding.Namespaces]::Normalize($defaultNamespace + "." + [System.IO.Path]::GetDirectoryName($outputPath).Replace([System.IO.Path]::DirectorySeparatorChar, "."))
$modelTypeNamespace = [T4Scaffolding.Namespaces]::GetNamespace($foundModelType.FullName)

Add-ProjectItemViaTemplate $outputPath -Template PresentationStubTemplate -Model @{  
		ExampleValue = $foundModelType.Name
		ModelType = [MarshalByRefObject]$foundModelType;
		ModelNamePascalCase = $modelNamePascalCase;
		PrimaryKey = [string]$primaryKey; 
		DefaultNamespace = $defaultNamespace; 
		ModelStubNamespace = $ModelStubNamespace; 
		ModelTypeNamespace = $modelTypeNamespace; 
		ModelTypePluralized = [string]$modelTypePluralized; 
		DbContextNamespace = $foundDbContextType.Namespace.FullName;
		DbContextType = [MarshalByRefObject]$foundDbContextType;
		RelatedEntities = $relatedEntities;
	} `
	-SuccessMessage "Added PresentationStub output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

$outputPath = Join-Path Models ($modelNamePascalCase + "/" + $modelNamePascalCase + "FormStub")
# We don't create areas here, so just ensure that if you specify one, it already exists
if ($Area) {
	$areaPath = Join-Path Areas $Area
	if (-not (Get-ProjectItem $areaPath -Project $Project)) {
		Write-Error "Cannot find area '$Area'. Make sure it exists already."
		return
	}
	$outputPath = Join-Path $areaPath $outputPath
}

Add-ProjectItemViaTemplate $outputPath -Template FormStubTemplate -Model @{  
		ExampleValue = $foundModelType.Name
		ModelType = [MarshalByRefObject]$foundModelType;
		ModelNamePascalCase = $modelNamePascalCase;
		PrimaryKey = [string]$primaryKey; 
		DefaultNamespace = $defaultNamespace; 
		ModelStubNamespace = $ModelStubNamespace; 
		ModelTypeNamespace = $modelTypeNamespace; 
		ModelTypePluralized = [string]$modelTypePluralized; 
		DbContextNamespace = $foundDbContextType.Namespace.FullName;
		DbContextType = [MarshalByRefObject]$foundDbContextType;
		RelatedEntities = $relatedEntities;
	} `
	-SuccessMessage "Added FormStub output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force



$outputPath = Join-Path Models ($modelNamePascalCase + "/" + $modelNamePascalCase + "ImportExcelStub")
# We don't create areas here, so just ensure that if you specify one, it already exists
if ($Area) {
	$areaPath = Join-Path Areas $Area
	if (-not (Get-ProjectItem $areaPath -Project $Project)) {
		Write-Error "Cannot find area '$Area'. Make sure it exists already."
		return
	}
	$outputPath = Join-Path $areaPath $outputPath
}

Add-ProjectItemViaTemplate $outputPath -Template ImportExcelStubTemplate -Model @{  
		ExampleValue = $foundModelType.Name
		ModelType = [MarshalByRefObject]$foundModelType;
		ModelNamePascalCase = $modelNamePascalCase;
		PrimaryKey = [string]$primaryKey; 
		DefaultNamespace = $defaultNamespace; 
		ModelStubNamespace = $ModelStubNamespace; 
		ModelTypeNamespace = $modelTypeNamespace; 
		ModelTypePluralized = [string]$modelTypePluralized; 
		DbContextNamespace = $foundDbContextType.Namespace.FullName;
		DbContextType = [MarshalByRefObject]$foundDbContextType;
		RelatedEntities = $relatedEntities;
	} `
	-SuccessMessage "Added ImportExcelStub output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force



$outputPath = Join-Path Controllers ($modelNamePascalCase + "Controller")
# We don't create areas here, so just ensure that if you specify one, it already exists
if ($Area) {
	$areaPath = Join-Path Areas $Area
	if (-not (Get-ProjectItem $areaPath -Project $Project)) {
		Write-Error "Cannot find area '$Area'. Make sure it exists already."
		return
	}
	$outputPath = Join-Path $areaPath $outputPath
}

$ControllerNamespace = [T4Scaffolding.Namespaces]::Normalize($defaultNamespace + "." + [System.IO.Path]::GetDirectoryName($outputPath).Replace([System.IO.Path]::DirectorySeparatorChar, "."))

Add-ProjectItemViaTemplate $outputPath -Template ControllerTemplate -Model @{  
		ExampleValue = $foundModelType.Name
		ModelType = [MarshalByRefObject]$foundModelType;
		ModelNamePascalCase = $modelNamePascalCase;
		PrimaryKey = [string]$primaryKey; 
		DefaultNamespace = $defaultNamespace; 
		ModelStubNamespace = $ModelStubNamespace; 
		ControllerNamespace = $ControllerNamespace; 
		ModelTypeNamespace = $modelTypeNamespace; 
		ModelTypePluralized = [string]$modelTypePluralized; 
		DbContextNamespace = $foundDbContextType.Namespace.FullName;
		DbContextType = [MarshalByRefObject]$foundDbContextType;
		RelatedEntities = $relatedEntities;
	} `
	-SuccessMessage "Added Controller output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force





$outputPath = Join-Path Views ($modelNamePascalCase + "/" + "Index")
# We don't create areas here, so just ensure that if you specify one, it already exists
if ($Area) {
	$areaPath = Join-Path Areas $Area
	if (-not (Get-ProjectItem $areaPath -Project $Project)) {
		Write-Error "Cannot find area '$Area'. Make sure it exists already."
		return
	}
	$outputPath = Join-Path $areaPath $outputPath
}

Add-ProjectItemViaTemplate $outputPath -Template IndexViewTemplate -Model @{  
		ExampleValue = $foundModelType.Name
		ModelType = [MarshalByRefObject]$foundModelType;
		ModelNamePascalCase = $modelNamePascalCase;
		PrimaryKey = [string]$primaryKey; 
		ControllerNamespace = $ControllerNamespace;
		ModelTypePluralized = [string]$modelTypePluralized; 
		DbContextNamespace = $foundDbContextType.Namespace.FullName;
		DbContextType = [MarshalByRefObject]$foundDbContextType;
		RelatedEntities = $relatedEntities;
	} `
	-SuccessMessage "Added Controller output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force


$outputPath = Join-Path Views ($modelNamePascalCase + "/" + "Form")
# We don't create areas here, so just ensure that if you specify one, it already exists
if ($Area) {
	$areaPath = Join-Path Areas $Area
	if (-not (Get-ProjectItem $areaPath -Project $Project)) {
		Write-Error "Cannot find area '$Area'. Make sure it exists already."
		return
	}
	$outputPath = Join-Path $areaPath $outputPath
}

Add-ProjectItemViaTemplate $outputPath -Template FormViewTemplate -Model @{  
		ExampleValue = $foundModelType.Name
		ModelType = [MarshalByRefObject]$foundModelType;
		ModelNamePascalCase = $modelNamePascalCase;
		PrimaryKey = [string]$primaryKey; 
		ModelStubNamespace = $ModelStubNamespace; 
		ControllerNamespace = $ControllerNamespace;
		ModelTypePluralized = [string]$modelTypePluralized; 
		DbContextNamespace = $foundDbContextType.Namespace.FullName;
		DbContextType = [MarshalByRefObject]$foundDbContextType;
		RelatedEntities = $relatedEntities;
	} `
	-SuccessMessage "Added Controller output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force


	$outputPath = Join-Path Views ($modelNamePascalCase + "/" + "ImportExcelForm")
# We don't create areas here, so just ensure that if you specify one, it already exists
if ($Area) {
	$areaPath = Join-Path Areas $Area
	if (-not (Get-ProjectItem $areaPath -Project $Project)) {
		Write-Error "Cannot find area '$Area'. Make sure it exists already."
		return
	}
	$outputPath = Join-Path $areaPath $outputPath
}

Add-ProjectItemViaTemplate $outputPath -Template ImportExcelViewTemplate -Model @{  
		ExampleValue = $foundModelType.Name
		ModelType = [MarshalByRefObject]$foundModelType;
		ModelNamePascalCase = $modelNamePascalCase;
		PrimaryKey = [string]$primaryKey; 
		ModelStubNamespace = $ModelStubNamespace; 
		ControllerNamespace = $ControllerNamespace;
		ModelTypePluralized = [string]$modelTypePluralized; 
		DbContextNamespace = $foundDbContextType.Namespace.FullName;
		DbContextType = [MarshalByRefObject]$foundDbContextType;
		RelatedEntities = $relatedEntities;
	} `
	-SuccessMessage "Added ImportExcelViewTemplate output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force
