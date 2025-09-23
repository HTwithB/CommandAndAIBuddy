resource "google_compute_global_address" "lb_ip" {
  name = "${var.lb_name}-lb-static-ip"
}

resource "google_compute_instance_group" "unmanaged_ig" {
  name      = "${var.lb_name}-instance-group"
  zone      = var.zone
  instances = var.instance_ids

  named_port {
    name = "http"
    port = 80
  }
}

resource "google_compute_health_check" "http_health_check" {
  name                = "${var.lb_name}-http-basic-check"
  check_interval_sec  = 5
  timeout_sec         = 5
  healthy_threshold   = 2
  unhealthy_threshold = 2
  http_health_check {
    request_path = "/healthz"
    port         = 80
  }
}

resource "google_compute_backend_service" "backend_service" {
  name        = "${var.lb_name}-backend-service"
  protocol    = "HTTP"
  port_name   = "http"
  timeout_sec = 30

  backend {
    group = google_compute_instance_group.unmanaged_ig.id
  }

  health_checks = [google_compute_health_check.http_health_check.id]
}

resource "google_compute_url_map" "url_map" {
  name            = "${var.lb_name}-lb-url-map"
  default_service = google_compute_backend_service.backend_service.id
}

resource "google_compute_target_https_proxy" "https_proxy" {
  name             = "${var.lb_name}-https-proxy"
  url_map          = google_compute_url_map.url_map.id
  ssl_certificates = [google_compute_managed_ssl_certificate.lb_cert.id]
}

resource "google_compute_global_forwarding_rule" "forwarding_rule" {
  name       = "${var.lb_name}-https-forwarding-rule"
  target     = google_compute_target_https_proxy.https_proxy.id
  ip_address = google_compute_global_address.lb_ip.address
  port_range = "443"
}

resource "google_compute_url_map" "url_map_redirect" {
  name = "${var.lb_name}-http-to-https-redirect"
  default_url_redirect {
    https_redirect         = true
    strip_query            = false
    redirect_response_code = "MOVED_PERMANENTLY_DEFAULT"
  }
}

resource "google_compute_target_http_proxy" "http_proxy" {
  name    = "${var.lb_name}-http-proxy"
  url_map = google_compute_url_map.url_map_redirect.id
}

resource "google_compute_global_forwarding_rule" "forwarding_rule_http" {
  name       = "${var.lb_name}-http-forwarding-rule"
  target     = google_compute_target_http_proxy.http_proxy.id
  ip_address = google_compute_global_address.lb_ip.address
  port_range = "80"
}