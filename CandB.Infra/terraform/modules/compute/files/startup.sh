#!/bin/bash

# スクリプトがエラーで停止した場合に即座に終了させる
set -e

# 1. パッケージリストを更新し、Nginxをインストール
# -y フラグは、すべての確認プロンプトに "yes" で自動応答します
echo "Updating package lists and installing Nginx..."
apt-get update
apt-get install -y nginx

# 2. デフォルトのNginx設定ファイルをバックアップ
echo "Backing up default Nginx configuration..."
mv /etc/nginx/nginx.conf /etc/nginx/nginx.conf.backup

# 3. 提供された設定内容で新しいnginx.confを作成
# ヒアドキュメント(<<'EOF')を使用して設定ファイルを書き込みます
echo "Creating new Nginx configuration file..."
cat << 'EOF' > /etc/nginx/nginx.conf
worker_processes auto;

events {
    worker_connections 1024;
}

http {
    include /etc/nginx/mime.types;
    default_type application/octet-stream;

    server {
        listen 80;
        server_name localhost;

        resolver 8.8.8.8;

        location /healthz {
            return 200 'OK';
            add_header Content-Type text/plain;
        }

        location = / {
            proxy_pass https://storage.googleapis.com/candb-473008-game-bucket/webfront/index.html;

            proxy_set_header Host storage.googleapis.com;
            proxy_ssl_server_name on;
        }

        location / {
            proxy_pass https://storage.googleapis.com/candb-473008-game-bucket/webfront/;
            proxy_set_header Host storage.googleapis.com;
            proxy_ssl_server_name on;
        }

        location = /mac/download {
            proxy_pass https://storage.googleapis.com/candb-473008-game-bucket/mac/candb.zip;
            proxy_set_header Host storage.googleapis.com;
            proxy_ssl_server_name on;
            add_header Content-Disposition 'attachment; filename="candb.zip"';
        }

        location = /windows/download {
            proxy_pass https://storage.googleapis.com/candb-473008-game-bucket/windows/candb.zip;
            proxy_set_header Host storage.googleapis.com;
            proxy_ssl_server_name on;
            add_header Content-Disposition 'attachment; filename="candb.zip"';
        }

        location = /mac {
            proxy_pass https://storage.googleapis.com/candb-473008-game-bucket/mac/installer.pkg;
            proxy_set_header Host storage.googleapis.com;
            proxy_ssl_server_name on;
            add_header Content-Disposition 'attachment; filename="installer.pkg"';
        }
    }
}
EOF

# 4. Nginxサービスを有効化し、再起動して新しい設定を適用
# systemctl enable: OS起動時にNginxが自動で起動するように設定
# systemctl restart: 新しい設定ファイルを読み込んでNginxを再起動
echo "Enabling and restarting Nginx service..."
systemctl enable nginx
systemctl restart nginx

echo "Startup script finished successfully!"