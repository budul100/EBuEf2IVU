docker build -t ebuef2ivucrew-image -f ./Programs/EBuEf2IVUCrew/Dockerfile .

docker run -v /mnt/c/Users/mgr/Entwicklung/EBuEf2IVU/Programs/EBuEf2IVUCrew/ebuef2ivucrew-settings.example.xml:/ebuef2ivucrew-settings.xml -t ebuef2ivucrew-image -s /ebuef2ivucrew-settings.xml
