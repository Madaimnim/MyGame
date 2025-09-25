#!/bin/bash
cd /d/Practice/TowerDefenseGame

# 取得當前日期與時間（格式：YYYY-MM-DD HH:MM:SS）
current_time=$(date +"%Y-%m-%d %H:%M:%S")

# 自動化 Git 流程
git add .
git commit -m "更新專案 - $current_time"
git push origin main

# 避免腳本結束後視窗馬上關閉
read -p "按任意鍵繼續..."