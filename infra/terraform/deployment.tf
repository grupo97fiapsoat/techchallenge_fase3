resource "kubernetes_deployment" "fastfood_api" {
  metadata {
    name = "fastfood-api"
    labels = {
      app = "fastfood-api"
    }
  }

  spec {
    replicas = 1

    selector {
      match_labels = {
        app = "fastfood-api"
      }
    }

    template {
      metadata {
        labels = {
          app = "fastfood-api"
        }
      }

      spec {
        container {
          name  = "fastfood-api"
          image = "techchallenge_fase1-api:latest"
image_pull_policy = "Never"

          resources {
            limits = {
              cpu    = "500m"
              memory = "256Mi"
            }
            requests = {
              cpu    = "250m"
              memory = "128Mi"
            }
          }

          port {
            container_port = 8080
          }

          env {
            name = "ASPNETCORE_ENVIRONMENT"
            value_from {
              config_map_key_ref {
                name = kubernetes_config_map.app_config.metadata[0].name
                key  = "ASPNETCORE_ENVIRONMENT"
              }
            }
          }

          env {
            name = "DB_PASSWORD"
            value_from {
              secret_key_ref {
                name = kubernetes_secret.db_credentials.metadata[0].name
                key  = "DB_PASSWORD"
              }
            }
          }

          env {
            name = "ConnectionStrings__DefaultConnection"
            value = "Server=${kubernetes_config_map.app_config.data["DB_SERVER"]};Database=${kubernetes_config_map.app_config.data["DB_NAME"]};User Id=${kubernetes_config_map.app_config.data["DB_USER"]};Password=$(DB_PASSWORD);TrustServerCertificate=true;"
          }
        }
      }
    }
  }
}