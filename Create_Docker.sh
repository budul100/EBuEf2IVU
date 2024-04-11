# Get into the folder and run script: 	
#
# cd /mnt/c/Users/mgr/Entwicklung/EBuEf2IVU
# docker login git.tu-berlin.de:5000
# sh Create_Docker.sh

echo
echo "Build ebuef2ivucrew"
echo

docker buildx build --network=host -t git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivucrew -f ./Programs/EBuEf2IVUCrew/Dockerfile .

docker push git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivucrew

echo
echo "Build ebuef2ivupath"
echo

docker buildx build --network=host -t git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivupath -f ./Programs/EBuEf2IVUPath/Dockerfile .

docker push git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivupath

echo
echo "Build ebuef2ivuvehicle"
echo

docker buildx build --network=host -t git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivuvehicle -f ./Programs/EBuEf2IVUVehicle/Dockerfile .

docker push git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivuvehicle

echo
echo "Create tar file"
echo

docker save --output ebuef2ivu.tar git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivucrew git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivupath git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivuvehicle

echo
echo "Copy tar file"
echo

cp ebuef2ivu.tar /mnt/c/Users/mgr/Dropbox/Public/EBuEf