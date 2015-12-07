# thisistracer
이거슨 업로드한 사진의 gps 태그를 읽어와 googlemap에 표시  
파일 저장 : Azure BlobStorage  
데이터 저장 : ~~Azure DocumentDB~~ , Azure Table Storage로 변경 (2015.12.08)  

## 사용 lib
1. Microsoft.WindowsAzure.Storage
2. google map
3. bootstrap
4. jQuery
5. Azure DocumentDB

## 참고 링크
1. [사진파일에서 gps 태그 가져오기](http://stackoverflow.com/questions/4983766/getting-gps-data-from-an-images-exif-in-c-sharp)
2. [Azure Blob Storage 기본 사용법](https://azure.microsoft.com/ko-kr/documentation/articles/storage-dotnet-how-to-use-blobs/)
3. [Azure Blob Storage 사용 예제](http://www.codeproject.com/Articles/490178/How-to-Use-Azure-Blob-Storage-with-Azure-Web-Sites)
4. [Customize GoogleMap InfoWindow](http://en.marnoto.com/2014/09/5-formas-de-personalizar-infowindow.html)
5. [Image 파일의 EXIF 값으로 이미지 회전](http://stackoverflow.com/questions/6222053/problem-reading-jpeg-metadata-orientation)
6. [Azure에 배포 시 민감한 정보 제외하기](http://www.hanselman.com/blog/HowToKeepYourASPNETDatabaseConnectionStringsSecureWhenDeployingToAzureFromSource.aspx)
7. [로컬 환경과 배포환경의 web.config 설정](https://azure.microsoft.com/ko-kr/blog/windows-azure-web-sites-how-application-strings-and-connection-strings-work/)

## ToDo
1. Upload Form  
2. ~~Social Login~~ (2015.10.13)
3. ~~User 별 container 생성 후 Upload~~ (2015.10.13)
4. 30일 이상 미활동 container/User Data 삭제
5. 사진 List-up (순서변경, 메타데이터 편집)
6. DocumentDB 적용 (2015.11.13)
