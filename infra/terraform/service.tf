resource "kubernetes_service" "fastfood_api" {
  metadata {
    name = "fastfood-api-service"
  }

  spec {
    selector = {
      app = kubernetes_deployment.fastfood_api.metadata[0].labels.app
    }

    port {
      port        = 80
      target_port = 8080
      node_port   = 30080
    }

    type = "NodePort"
  }
}