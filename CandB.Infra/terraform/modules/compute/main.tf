resource "google_compute_instance" "main" {
  project                   = var.project_id
  name                      = var.instance_name
  machine_type              = "e2-small"
  zone                      = var.zone
  allow_stopping_for_update = true

  boot_disk {
    initialize_params {
      image = var.os_image
    }
  }

  network_interface {
    network = "default"
    access_config {}
  }

  service_account {
    email  = var.service_account_email
    scopes = ["cloud-platform"]
  }

  metadata_startup_script = file("${path.module}/files/startup.sh")

  tags = ["${var.instance_name}-backend", "http-server"]
}

resource "google_compute_firewall" "allow_lb_and_healthcheck" {
  name    = "${var.instance_name}-allow-lb-and-healthcheck"
  network = "default"

  allow {
    protocol = "tcp"
    ports    = ["80"]
  }

  target_tags = ["${var.instance_name}-backend"]

  # https://cloud.google.com/load-balancing/docs/firewall-rules
  source_ranges = ["130.211.0.0/22", "35.191.0.0/16"]
}

resource "google_compute_firewall" "allow_egress_http_https" {
  project = var.project_id
  name    = "${var.instance_name}-allow-egress-http-https"
  network = "default" # デフォルトVPCを指定

  description = "Allow egress traffic on TCP ports 80 and 443"

  direction = "EGRESS"

  target_tags = ["${var.instance_name}-backend"]

  destination_ranges = ["0.0.0.0/0"]

  allow {
    protocol = "tcp"
    ports    = ["80", "443"]
  }
}

resource "google_compute_firewall" "allow_http" {
  project = var.project_id
  name    = "${var.instance_name}-allow-http-traffic"
  network = "default" # VPCネットワーク名

  description = "Allow incoming HTTP traffic"

  direction = "INGRESS"

  source_ranges = ["0.0.0.0/0"]

  target_tags = ["${var.instance_name}-backend"]

  allow {
    protocol = "tcp"
    ports    = ["80"]
  }
}
