#!/bin/bash

# Script para gerar certificados de desenvolvimento
# Este script cria certificados autoassinados para uso em desenvolvimento

echo "🔐 Gerando certificados de desenvolvimento para HTTPS..."

# Criar diretório para certificados se não existir
mkdir -p ./certs

# Gerar chave privada
openssl genrsa -out ./certs/fastfood-dev.key 2048

# Gerar certificado autoassinado
openssl req -new -x509 -key ./certs/fastfood-dev.key -out ./certs/fastfood-dev.crt -days 365 -subj "/C=BR/ST=SP/L=SaoPaulo/O=FastFood/OU=Development/CN=localhost"

# Gerar arquivo PFX para .NET
openssl pkcs12 -export -out ./certs/fastfood-dev.pfx -inkey ./certs/fastfood-dev.key -in ./certs/fastfood-dev.crt -password pass:fastfood123

# Definir permissões
chmod 600 ./certs/*

echo "✅ Certificados gerados com sucesso!"
echo "📁 Localização: ./certs/"
echo "🔑 Senha do certificado PFX: fastfood123"
echo ""
echo "⚠️  IMPORTANTE: Este é um certificado autoassinado para DESENVOLVIMENTO apenas!"
echo "   Para produção, use certificados válidos de uma CA confiável."
