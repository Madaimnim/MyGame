#!/bin/bash
set -e  # 若有任何指令出錯立即停止

# 🔍 自動切換到這個腳本所在資料夾（確保無論在哪台電腦都能正確執行）
cd "$(dirname "$0")"

# 🕓 取得當前日期與時間
current_time=$(date +"%Y-%m-%d %H:%M:%S")

# 分支名稱
branch_name="EditorVersion2021.3.20f1"

# 檢查並切換分支
echo "🔄 檢查分支狀態..."
git fetch origin
if git show-ref --verify --quiet "refs/heads/$branch_name"; then
    git checkout $branch_name
else
    echo "✨ 本地沒有此分支，建立新的 $branch_name"
    git checkout -b $branch_name
fi

echo "🔄 從遠端同步最新版本 ($branch_name)..."
if ! git pull origin $branch_name; then
    echo "⚠️ 下載最新版本失敗，請先解決衝突或檢查網路。"
    read -n 1 -s -r -p "按下任意鍵關閉..."
    exit 1
fi

# 加入變更
git add -A

# 判斷是否有變更
if git diff --quiet && git diff --cached --quiet; then
    echo "沒有檔案變更，不會建立 commit。"
else
    read -p "請輸入 commit 訊息（預設：更新 $branch_name 分支）：" user_msg
    if [[ -z "${user_msg// }" ]]; then
        user_msg="更新 $branch_name 分支"
    fi

    git commit -m "$user_msg - $current_time"
    git push origin $branch_name

    echo "✅ 已推送到遠端分支：$branch_name"
    echo "📌 最新 Commit："
    git log -1 --oneline --decorate
fi

read -n 1 -s -r -p "按下任意鍵關閉..."
exit
