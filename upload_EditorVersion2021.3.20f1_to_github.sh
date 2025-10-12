#!/bin/bash
cd /d/Practice/TowerDefenseGame

# 取得當前日期與時間（格式：YYYY-MM-DD HH:MM:SS）
current_time=$(date +"%Y-%m-%d %H:%M:%S")

# 分支名稱（這是你指定的）
branch_name="EditorVersion2021.3.20f1"

# 檢查並切換到目標分支
echo "🔄 檢查分支狀態..."
git fetch origin
if git show-ref --verify --quiet "refs/heads/$branch_name"; then
    git checkout $branch_name
else
    echo "✨ 本地沒有此分支，建立新的 $branch_name"
    git checkout -b $branch_name
fi

echo "🔄 從遠端同步最新版本 ($branch_name)..."
git pull origin $branch_name

# 檢查 pull 是否成功
if [ $? -ne 0 ]; then
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
    read -p "請輸入 commit 訊息（預設：更新 EditorVersion2021.3.20f1 分支）：" user_msg
    if [[ -z "${user_msg// }" ]]; then
        user_msg="更新 EditorVersion2021.3.20f1 分支"
    fi

    git commit -m "$user_msg - $current_time"
    git push origin $branch_name

    echo "✅ 已推送到遠端分支：$branch_name"
    echo "📌 最新 Commit："
    git log -1 --oneline --decorate
fi

read -n 1 -s -r -p "按下任意鍵關閉..."
exit
