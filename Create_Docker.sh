# Get into the folder and run script: 	
#
# cd /mnt/c/Users/mgr/Entwicklung/EBuEf2IVU
# sh Create_Docker.sh

echo
echo "Build ebuef2ivucrew"
echo

docker buildx build --network=host -t "ebuef2ivucrew" -f ./Programs/EBuEf2IVUCrew/Dockerfile .

echo
echo "Build ebuef2ivupath"
echo

docker buildx build --network=host -t git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivupath -f ./Programs/EBuEf2IVUCrew/Dockerfile .

echo
echo "Build ebuef2ivuvehicle"
echo

docker buildx build --network=host -t "ebuef2ivuvehicle" -f ./Programs/EBuEf2IVUVehicle/Dockerfile .

echo
echo "Create tar file"
echo

docker save --output ebuef2ivu.tar ebuef2ivucrew ebuef2ivupath ebuef2ivuvehicle