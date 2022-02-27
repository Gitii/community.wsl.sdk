generate-coverage.cmd

rmdir /S /Q CoverageReport
reportgenerator "-reports:.\CoverageResults\coverage.net6.0.cobertura.xml" "-targetdir:CoverageReport" "-reporttypes:Html;HtmlSummary"
rmdir /S /Q CoverageResults
