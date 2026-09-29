# bannertool

A tool for creating 3DS banners.

## How to build (Using Docker)

```bash
docker build -t bannertool-builder .
docker run --name bannertool-builder bannertool-builder
docker cp bannertool-builder:/bannertool/output/bannertool.zip .
```
