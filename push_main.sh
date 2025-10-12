#!/bin/bash
set -e  # 若有任何指令出錯立即停止

# 🔍 自動切換到這個腳本所在資料夾（確保無論在哪台電腦都能正確執行）
cd "$(dirname "$0")"

# 🕓 取得當前日期與時間
current_time=$(date +"%Y-%m-%d %H:%M:%S")

echo "🔄 先從遠端下載最新版本 (git pull)..."
if ! git pull origin main; then
    echo "⚠️ 下載最新版本失敗，請先解決衝突或檢查網路。"
    read -n 1 -s -r -p "按下任意鍵關閉..."
    exit 1
fi

# ➕ 加入所有變更
git add -A

# 🔍 判斷是否有變更
if git diff --quiet && git diff --cached --quiet; then
    echo "沒有檔案變更，不會建立 commit。"
else
    read -p "請輸入 commit 訊息（預設：更新專案）：" user_msg
    if [[ -z "${user_msg// }" ]]; then
        user_msg="更新專案"
    fi

    git commit -m "$user_msg - $current_time"
    git push origin main

    echo "✅ 已推送到遠端 main 分支。"
    echo "📌 最新 Commit："
    git log -1 --oneline --decorate
fi

read -n 1 -s -r -p "按下任意鍵關閉..."
exit
