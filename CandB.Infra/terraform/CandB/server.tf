module "server" {
  source = "../modules/compute"

  project_id            = data.google_project.project.project_id
  instance_name         = "candb-instance"
  zone                  = "${local.project_region}-a"
  service_account_email = module.bucket.signer_service_account_email

  bucket = {
    name                              = module.bucket.name
    hmac_access_key                   = module.bucket.hmac_access_key
    hmac_secret_key_secret_manager_id = module.bucket.hmac_secret_key_secret_manager_id
  }
}

module "lb" {
  source       = "../modules/lb"
  zone         = "${local.project_region}-a"
  lb_name      = "candb-lb"
  instance_ids = [module.server.instance_id]

  dns = {
    domain       = "site.${local.domain}"
    managed_zone = google_dns_managed_zone.hanachiru_net.name
  }
}
