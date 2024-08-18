dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:podclave.core.test/TestResults/**/*.cobertura.xml -targetDir:coveragereport
