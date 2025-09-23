locals {
  group_email = "candb-developers@googlegroups.com"
  group_roles = [
    "roles/viewer",
  ]
}

resource "google_project_iam_member" "group" {
  for_each = toset(local.group_roles)

  project = data.google_project.project.project_id
  role    = each.value
  member  = "group:${local.group_email}"
}
