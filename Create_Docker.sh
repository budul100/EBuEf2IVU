echo
echo "Build ebuef2ivucrew"
echo

docker buildx build --network=host -t "ebuef2ivucrew" -f ./Programs/EBuEf2IVUCrew/Dockerfile .

echo
echo "Build ebuef2ivupath"
echo

docker buildx build --network=host -t "ebuef2ivupath" -f ./Programs/EBuEf2IVUPath/Dockerfile .

echo
echo "Build ebuef2ivuvehicle"
echo

docker buildx build --network=host -t "ebuef2ivuvehicle" -f ./Programs/EBuEf2IVUVehicle/Dockerfile .

echo
echo "Create tar file"
echo

docker save --output ebuef2ivu.tar ebuef2ivucrew ebuef2ivupath ebuef2ivuvehicle