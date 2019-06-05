FROM mcr.microsoft.com/dotnet/core/sdk:3.0
USER root
LABEL maintainer="Ivan Nečas"

# the mono dependency for paket, locales for getting rid of warinings and
# vim for convenience
RUN apt update && apt install mono-devel locales vim -y && rm -rf /var/lib/apt/lists/*
# To get rid perl locale warnings
RUN echo 'en_US.UTF-8 UTF-8' > /etc/locale.gen && /usr/sbin/locale-gen

ADD . /app
WORKDIR /app

# Fore some reason, the first restore exits with non-zero code
RUN dotnet restore; dotnet restore
RUN dotnet run -p src/FSharpSqlite.Migrate

ENTRYPOINT ["/bin/bash"]
