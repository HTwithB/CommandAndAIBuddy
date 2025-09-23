resource "google_dns_record_set" "a_record" {
  name = "${var.dns.domain}."
  type = "A"
  ttl  = 300

  # ロードバランサーのIPアドレスを指定
  managed_zone = var.dns.managed_zone
  rrdatas      = [google_compute_global_address.lb_ip.address]
}

resource "google_compute_managed_ssl_certificate" "lb_cert" {
  name = "${var.lb_name}-gateway-cert"
  managed {
    domains = [
      "${var.dns.domain}",
    ]
  }
}