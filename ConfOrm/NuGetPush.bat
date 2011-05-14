rem The parameter is your NuGet access key 
nuget pack ConfORM.nuspec
nuget push -source http://packages.nuget.org/v1/ ConfOrm.1.0.1.5.nupkg %1 
