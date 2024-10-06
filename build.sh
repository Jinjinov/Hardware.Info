#!/usr/bin/env bash

set -e

while [[ $# -gt 0 ]]; do
  case $1 in
    -v|--version)
      VERSION="$2"
      shift # past argument
      shift # past value
      ;;
    -t|--target)
      TFM="$2"
      shift # past argument
      shift # past value
      ;;
    --aot)
      AOT=YES
      shift # past argument
      ;;
    -*|--*)
      echo "Unknown option $1"
      exit 1
      ;;
    *)
      echo "Unknown argument $1"
      exit 1
      ;;
  esac
done

if [[ -z "$TFM" ]];
then
  echo "Option -t|--target is missing"
  exit 1;
fi

if [[ -z "$VERSION" ]];
then
  echo "Option -v|--version is missing"
  exit 1;
fi

if [[ "$AOT" = "YES" ]];
then
  PROJECT="Hardware.Info.Aot/Hardware.Info.Aot.csproj"
else
  PROJECT="Hardware.Info/Hardware.Info.csproj"
fi

echo "Restoring..."
dotnet restore \
  "$PROJECT" \
  --verbosity quiet \
  /p:Warn=0 \
  -p:TargetFrameworks="$TFM"

echo "Building..."
dotnet build --no-restore \
  "$PROJECT" \
  --configuration Release \
  --verbosity quiet \
  /p:Warn=0 \
  -p:Version="$VERSION" \
  -p:TargetFrameworks="$TFM"

echo "Package project..."
mkdir -p ./dist
dotnet pack --no-restore \
  "$PROJECT" \
  --configuration Release \
  --verbosity quiet \
  /p:Warn=0 \
  -p:Version="$VERSION" \
  -p:PackageVersion="$VERSION" \
  -p:TargetFrameworks="$TFM" \
  -o ./dist

echo "List of created artifacts:"
ls -al ./dist/*