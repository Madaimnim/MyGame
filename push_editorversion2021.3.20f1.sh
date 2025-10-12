#!/bin/bash
set -e  # 有錯誤就立即停止

# 🔍 自動切換到這個腳本所在的資料夾
cd "$(dirname "$0")"

# 🕓 取得當前日期與時間
current_time=$(date +"%Y-%m-%d %H:%M:%S")

# 分支名稱（固定綁定這個分支）
branch_name="EditorVersion2021.3.20f1"

# 🔒 防呆檢查：確認目前是否在正確分支
current_branch=$(git rev-parse --abbrev-ref HEAD)
if [ "$current_branch" != "$branch_name" ]; then
    echo "⚠️ 當前在分支：$current_branch"
    echo "👉 這個腳本只能在 $branch_name 分支使用。"
    echo "自動切換中..."
    git checkout "$branch_name"
fi

# 🔄 從遠端同步最新版本
echo "🔄 從遠端同步最新版本 ($branch_name)..."
if ! git pull origin "$branch_name"; then
    echo "⚠️ 下載最新版本失敗，請先解決衝突或檢查網路。"
    read -n 1 -s -r -p "按任意鍵關閉..."
    exit 1
fi

# ➕ 加入所有變更
git add -A

# 🔍 判斷是否有變更
if git diff --quiet && git diff --cached --quiet; then
    echo "沒有檔案變更，不會建立 commit。"
else
    read -p "請輸入 commit 訊息（預設：更新 $branch_name 分支）：" user_msg
    if [[ -z "${user_msg// }" ]]; then
        user_msg="更新 $branch_name 分支"
    fi

    # 💾 建立 commit 並推送
    git commit -m "$user_msg - $current_time"
    git push origin "$branch_name"

    echo "✅ 已推送到遠端分支：$branch_name"
    echo "📌 最新 Commit："
    git log -1 --oneline --decorate -n 1
fi

read -n 1 -s -r -p "按任意鍵關閉..."
exit
