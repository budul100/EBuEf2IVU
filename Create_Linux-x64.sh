export PATH="$PATH:/home/mgr/.dotnet/tools"

cd /mnt/c/Users/mgr/Entwicklung/EBuEf2IVU/Programs/$1

dotnet deb --configuration Release --framework $2 --runtime $3
dotnet msbuild /p:TargetFramework=$2 /p:RuntimeIdentifier=$3 /p:Configuration=Release /t:CreateDeb

# cd /mnt/c/Users/mgr/Entwicklung/EBuEf2IVU
# 
# docker buildx build --network=host -t "$1" -f ./Programs/$1/Dockerfile .
