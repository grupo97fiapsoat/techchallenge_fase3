resource "kubernetes_config_map" "app_config" {
  metadata {
    name = "app-config"
  }

  data = {
    ASPNETCORE_ENVIRONMENT = "Development"
    DB_SERVER              = "db,1433"
    DB_NAME                = "FastFood"
    DB_USER                = "sa"
  }
}