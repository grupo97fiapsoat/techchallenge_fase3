resource "kubernetes_secret" "db_credentials" {
  metadata {
    name = "db-credentials"
  }

  data = {
    DB_PASSWORD = base64encode("FastFood2025")
  }

  type = "Opaque"
}