FROM hub.nexdev.net/base/dotnet/core/sdk:6.0 AS builder

WORKDIR /src

COPY src/api/ONEbook.UM.Api/ONEbook.UM.Api.csproj ./ONEbook.UM.Api/ONEbook.UM.Api.csproj

WORKDIR /src/ONEbook.UM.Api
RUN dotnet nuget add source http://nuget.nexdev.net/api/v2
RUN dotnet restore -v n

COPY /src/api/ONEbook.UM.Api .

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0 AS base

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

# Don’t run as root
RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser
#

WORKDIR /app
COPY --from=builder /app .
ENV TZ="America/La_Paz"


ENTRYPOINT ["dotnet", "ONEbook.UM.Api.dll"]