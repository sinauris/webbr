#FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
#WORKDIR /app
#COPY /production ./
#ENTRYPOINT ["dotnet", "WebbrVue.dll"]
#-------------------------------------------------------------------------------
#FROM microsoft/dotnet:sdk AS build
#WORKDIR /app
#
## Copy csproj and restore as distinct layers
#COPY *.csproj ./
#RUN dotnet restore
#
## Copy everything else and build
#COPY . ./
#
## Требуется для выполнения скрипта сборки клиентской части
#ENV NODE_VERSION 8.9.4
#ENV NODE_DOWNLOAD_SHA 21fb4690e349f82d708ae766def01d7fec1b085ce1f5ab30d9bda8ee126ca8fc
#RUN curl -SL "https://nodejs.org/dist/v${NODE_VERSION}/node-v${NODE_VERSION}-linux-x64.tar.gz" --output nodejs.tar.gz \
#    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
#    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
#    && rm nodejs.tar.gz \
#    && ln -s /usr/local/bin/node /usr/local/bin/nodejs
#
#RUN dotnet publish -c Release -o out
#
## Build runtime image
#FROM microsoft/dotnet:aspnetcore-runtime
#WORKDIR /app
#COPY --from=build /app/out .
#ENTRYPOINT ["dotnet", "Webbr.dll"]
#------------------------------------------------------------------------------
ARG NODE_IMAGE=node:11.10

FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
EXPOSE 80


FROM microsoft/dotnet:sdk AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore


FROM ${NODE_IMAGE} as node-build
WORKDIR /src
COPY ./ .
RUN npm install
COPY ./lib/froala-editor-self ./src/node_modules/froala-editor
RUN npm run build


FROM build AS publish
RUN dotnet publish -c Release -o out


FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build /app/out .
COPY --from=node-build /src/dist ./ClientApp/dist
ENTRYPOINT ["dotnet", "Webbr.dll"]