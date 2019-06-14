FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY Crawler/*.csproj ./
COPY /API/HouseMap.Dao/*.csproj /API/HouseMap.Dao/HouseMap.Dao.csproj
RUN dotnet restore

# copy everything else and build
COPY . .
RUN dotnet publish Crawler -c Release -o out

# build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime


WORKDIR /app
COPY --from=build-env /app/Crawler/out .
ENTRYPOINT ["dotnet", "HouseMap.Crawler.dll"]