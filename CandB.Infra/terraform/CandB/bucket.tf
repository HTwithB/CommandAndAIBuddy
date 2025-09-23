module "bucket" {
  project_id    = local.project_id
  source        = "../modules/bucket"
  bucket_name   = "${data.google_project.project.project_id}-game-bucket"
  location      = local.project_region
  storage_class = "STANDARD"
  bucket_admins = [
    "group:candb-developers@googlegroups.com",
  ]
  use_signed_url = true
}
