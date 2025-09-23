locals {
  dns_zone      = local.project_id
  domain        = "candb.hanachiru.net"
  zone_dns_name = "${local.domain}."
}

resource "google_dns_managed_zone" "hanachiru_net" {
  name     = local.dns_zone
  dns_name = local.zone_dns_name

  dnssec_config {
    kind          = "dns#managedZoneDnsSecConfig"
    non_existence = "nsec3"
    state         = "on"

    default_key_specs {
      algorithm  = "rsasha256"
      key_length = 2048
      key_type   = "keySigning"
      kind       = "dns#dnsKeySpec"
    }
    default_key_specs {
      algorithm  = "rsasha256"
      key_length = 1024
      key_type   = "zoneSigning"
      kind       = "dns#dnsKeySpec"
    }
  }
}
