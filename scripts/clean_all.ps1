#######################
# Delete Junk

Get-ChildItem .. -Recurse -include 'bin', 'obj', 'packages', '_ReSharper.Caches', 'node_modules' |
  ForEach-Object {
    if ((Get-ChildItem $_.Parent.FullName | Where-Object{ $_.Name -Like "*.sln" -or $_.Name -Like "*.*proj" }).Length -gt 0) {
      Remove-Item $_ -recurse -force
      Write-Host deleted $_
    }
}
