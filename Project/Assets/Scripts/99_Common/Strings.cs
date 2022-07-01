using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Language
{
    English = 0, Korean, Japanese, Chinese
}

public enum StringKey
{
    InitializeConnectingMessage = 0,
    InitializeTextNickname,
    InitializeButtonGameStart,
    MainButtonOpenCreateRoomUI,
    MainButtonRoomListRefresh,
    MainButtonOpenCreateRoomUIInRoomList,
    MainButtonCreateRoom,
    MainButtonCloseCreateRoomUI,
    MainButtonDeveloper,
    MainButtonSettings,
    MainButtonExitGame,
    MainTextRoomCount,
    MainTextNoRoom,
    MainTextNoRoomNameInInputField,
    MainTextRoomNameTitle,
    InGameButtonResume,
    InGameButtonSettings,
    InGameButtonReconnect,
    InGameButtonExit,

    ItemHandWarmer,
    ItemGrilledFish,
    ItemFish,
    ItemEmptyBottle,
    ItemFullBottle,
    ItemUsingHandWarmer,
    ItemColdHandWarmer,
    ItemWood,

    ItemHandWarmerDescript,
    ItemGrilledFishDescript,
    ItemFishDescript,
    ItemEmptyBottleDescript,
    ItemFullBottleDescript,
    ItemUsingHandWarmerDescript,
    ItemColdHandWarmerDescript,
    ItemWoodDescript,

    InGameDieMessage,
    InGameMurderPlayer,
    InGameNoMurderPlayer,

    InGameDieMessageHungry,
    InGameDieMessageFalling,
    InGameDieMessageVote,
    InGameDieMessageNone,

    InGameMurderWin,
    InGamePlayerWin,
    InGameWorkNoMessage,
    InGameWorkTree,
    InGameWorkWater,
    InGameWorkFish,
    InGameWorkOpenVote,

    InGameWorkProcessTree,
    InGameWorkProcessWater,
    InGameWorkProcessFish,

    InGameMessageInventoryIsFull,
    InGameMessageNotExistItem,
    InGameMessageWorkingTryOtherAction,

    InGameMessageCompleteVote,
    InGameFinishVoteNone,
    InGameFinishVoteMurderTrue,
    InGameFinishVoteMurderFalse,
    InGameFinishVoteMurderCount,
}

public static class Strings
{
    private static Dictionary<StringKey, string> str = new Dictionary<StringKey, string>();

    public static void SetLanguage(Language value)
    {
        str.Clear();
        switch (value) {
            case Language.English:
                str.Add(StringKey.InitializeConnectingMessage, "Connecting to server...");
                str.Add(StringKey.InitializeTextNickname, "Write your nickname");
                str.Add(StringKey.InitializeButtonGameStart, "Play");
                str.Add(StringKey.MainButtonOpenCreateRoomUI, "Create");
                str.Add(StringKey.MainButtonRoomListRefresh, "Refresh");
                str.Add(StringKey.MainButtonOpenCreateRoomUIInRoomList, "Create Room");
                str.Add(StringKey.MainButtonCreateRoom, "Create");
                str.Add(StringKey.MainButtonCloseCreateRoomUI, "Cancel");
                str.Add(StringKey.MainButtonDeveloper, "Developer");
                str.Add(StringKey.MainButtonSettings, "Settings");
                str.Add(StringKey.MainButtonExitGame, "Exit game");
                str.Add(StringKey.MainTextRoomCount, "Room Count");
                str.Add(StringKey.MainTextNoRoom, "No Room");
                str.Add(StringKey.MainTextNoRoomNameInInputField, "Input room title");
                str.Add(StringKey.MainTextRoomNameTitle, "Room name");
                str.Add(StringKey.InGameButtonResume, "Resume");
                str.Add(StringKey.InGameButtonSettings, "Settings");
                str.Add(StringKey.InGameButtonReconnect, "Reconnect");
                str.Add(StringKey.InGameButtonExit, "Quit game");
                str.Add(StringKey.ItemHandWarmer, "Hand warmer");
                str.Add(StringKey.ItemUsingHandWarmer, "Hand warmer");
                str.Add(StringKey.ItemColdHandWarmer, "Hand warmer");
                str.Add(StringKey.ItemEmptyBottle, "Empty bottle");
                str.Add(StringKey.ItemFullBottle, "Full bottle");
                str.Add(StringKey.ItemFish, "Fish");
                str.Add(StringKey.ItemGrilledFish, "Hand warmer");
                str.Add(StringKey.ItemWood, "장작");
                str.Add(StringKey.ItemWoodDescript, "불이 잘 붙을 것 같은 장작이다.");
                str.Add(StringKey.ItemHandWarmerDescript, "사용하면 따뜻해질 것 같은 손난로다.");
                str.Add(StringKey.ItemUsingHandWarmerDescript, "사용해서 따뜻해진 손난로다.");
                str.Add(StringKey.ItemColdHandWarmerDescript, "다 식어버린 손난로다.");
                str.Add(StringKey.ItemEmptyBottleDescript, "물통이 비어있다.");
                str.Add(StringKey.ItemFullBottleDescript, "물이 가득 차있다.");
                str.Add(StringKey.ItemFishDescript, "방금 잡은 듯한 모습을 한 물고기다.");
                str.Add(StringKey.ItemGrilledFishDescript, "바싹 익혀서 노릇노릇 구워진 맛있는 생선이 되었다.");
                str.Add(StringKey.InGameDieMessage, "You were killed by {0}.");
                str.Add(StringKey.InGameMurderPlayer, "You are a killer");
                str.Add(StringKey.InGameNoMurderPlayer, "You are surviver");
                str.Add(StringKey.InGameDieMessageHungry, "death of starvation");
                str.Add(StringKey.InGameDieMessageFalling, "fell and died");
                str.Add(StringKey.InGameDieMessageVote, "vote die");
                str.Add(StringKey.InGameDieMessageNone, "Unknown");
                str.Add(StringKey.InGameMurderWin, "Murderers win");
                str.Add(StringKey.InGamePlayerWin, "Players win");

                str.Add(StringKey.InGameWorkNoMessage, "None");
                str.Add(StringKey.InGameWorkTree, "Tree");
                str.Add(StringKey.InGameWorkWater, "Water");
                str.Add(StringKey.InGameWorkFish, "Fish");
                str.Add(StringKey.InGameWorkOpenVote, "Report");

                str.Add(StringKey.InGameWorkProcessTree, "Tree");
                str.Add(StringKey.InGameWorkProcessWater, "Water");
                str.Add(StringKey.InGameWorkProcessFish, "Fish");

                str.Add(StringKey.InGameMessageInventoryIsFull, "Inventory has been full");
                str.Add(StringKey.InGameMessageNotExistItem, "You don't have a \"{0}\"");
                str.Add(StringKey.InGameMessageWorkingTryOtherAction, "You cannot try actions while working.");

                str.Add(StringKey.InGameMessageCompleteVote, "{0} has voted");
                str.Add(StringKey.InGameFinishVoteNone, "No one reported");
                str.Add(StringKey.InGameFinishVoteMurderTrue, "{0} was murderer");
                str.Add(StringKey.InGameFinishVoteMurderFalse, "{0} was not murderer");
                str.Add(StringKey.InGameFinishVoteMurderCount, "Remain murderer count : {0}");



                break;
            case Language.Korean:
                str.Add(StringKey.InitializeConnectingMessage, "서버 연결중...");
                str.Add(StringKey.InitializeTextNickname, "닉네임을 입력해주세요");
                str.Add(StringKey.InitializeButtonGameStart, "시작");

                str.Add(StringKey.MainButtonOpenCreateRoomUI, "만들기");
                str.Add(StringKey.MainButtonRoomListRefresh, "갱신");
                str.Add(StringKey.MainButtonOpenCreateRoomUIInRoomList, "방 만들기");
                str.Add(StringKey.MainButtonCreateRoom, "생성");
                str.Add(StringKey.MainButtonCloseCreateRoomUI, "취소");
                str.Add(StringKey.MainButtonDeveloper, "노예들");
                str.Add(StringKey.MainButtonSettings, "환경 설정");
                str.Add(StringKey.MainButtonExitGame, "게임 종료");
                str.Add(StringKey.MainTextRoomCount, "방 갯수");
                str.Add(StringKey.MainTextNoRoom, "생성된 방이 없습니다");
                str.Add(StringKey.MainTextNoRoomNameInInputField, "방 이름을 입력해주세요");
                str.Add(StringKey.MainTextRoomNameTitle, "방 이름");

                str.Add(StringKey.InGameButtonResume, "돌아가기");
                str.Add(StringKey.InGameButtonSettings, "설정");
                str.Add(StringKey.InGameButtonReconnect, "재접속");
                str.Add(StringKey.InGameButtonExit, "게임 이탈");
                str.Add(StringKey.ItemHandWarmer, "손난로");
                str.Add(StringKey.ItemUsingHandWarmer, "손난로");
                str.Add(StringKey.ItemColdHandWarmer, "손난로");
                str.Add(StringKey.ItemEmptyBottle, "빈 물통");
                str.Add(StringKey.ItemFullBottle, "물통");
                str.Add(StringKey.ItemFish, "물고기");
                str.Add(StringKey.ItemGrilledFish, "구운 물고기");
                str.Add(StringKey.ItemWood, "장작");
                str.Add(StringKey.ItemWoodDescript, "불이 잘 붙을 것 같은 장작이다.");
                str.Add(StringKey.ItemHandWarmerDescript, "사용하면 따뜻해질 것 같은 손난로다.");
                str.Add(StringKey.ItemUsingHandWarmerDescript, "사용해서 따뜻해진 손난로다.");
                str.Add(StringKey.ItemColdHandWarmerDescript, "다 식어버린 손난로다.");
                str.Add(StringKey.ItemEmptyBottleDescript, "물통이 비어있다.");
                str.Add(StringKey.ItemFullBottleDescript, "물이 가득 차있다.");
                str.Add(StringKey.ItemFishDescript, "방금 잡은 듯한 모습을 한 물고기다.");
                str.Add(StringKey.ItemGrilledFishDescript, "바싹 익혀서 노릇노릇 구워진 맛있는 생선이 되었다.");
                str.Add(StringKey.InGameDieMessage, "당신은 {0}에게 살해당했습니다.");
                str.Add(StringKey.InGameMurderPlayer, "들키지 않고 모두 죽이세요");
                str.Add(StringKey.InGameNoMurderPlayer, "무조건 생존하세요");
                str.Add(StringKey.InGameDieMessageHungry, "굶주림에 지쳐 사망했습니다");
                str.Add(StringKey.InGameDieMessageFalling, "높은 곳에서 떨어져 사망했습니다");
                str.Add(StringKey.InGameDieMessageVote, "투표로 인해 쫓겨났습니다.");
                str.Add(StringKey.InGameDieMessageNone, "Unknown");
                str.Add(StringKey.InGameMurderWin, "살인자에게 모두 당했습니다.");
                str.Add(StringKey.InGamePlayerWin, "살인자를 모두 처리했습니다.");

                str.Add(StringKey.InGameWorkNoMessage, "None");
                str.Add(StringKey.InGameWorkTree, "나무 구하기");
                str.Add(StringKey.InGameWorkWater, "물 구하기");
                str.Add(StringKey.InGameWorkFish, "낚시하기");
                str.Add(StringKey.InGameWorkOpenVote, "신고하기");

                str.Add(StringKey.InGameWorkProcessTree, "나무를 구하는 중입니다.");
                str.Add(StringKey.InGameWorkProcessWater, "물을 퍼고있습니다.");
                str.Add(StringKey.InGameWorkProcessFish, "물고기를 낚는중입니다.");

                str.Add(StringKey.InGameMessageInventoryIsFull, "아이템을 더 이상 구할 수 없습니다!");
                str.Add(StringKey.InGameMessageNotExistItem, "아이템이 없습니다. \"{0}\"");
                str.Add(StringKey.InGameMessageWorkingTryOtherAction, "작업 중에는 작업을 시도할 수 없습니다.");

                str.Add(StringKey.InGameMessageCompleteVote, "{0}님이 투표를 완료했습니다.");
                str.Add(StringKey.InGameFinishVoteNone, "아무도 내보내지 않았습니다.");
                str.Add(StringKey.InGameFinishVoteMurderTrue, "{0}님은 살인자였습니다.");
                str.Add(StringKey.InGameFinishVoteMurderFalse, "{0}님은 살인자가 아니였습니다.");
                str.Add(StringKey.InGameFinishVoteMurderCount, "살인자가 {0}명 있습니다.");
                break;
            case Language.Japanese:
                str.Add(StringKey.InitializeConnectingMessage, "サーバーに接続中...");
                str.Add(StringKey.InitializeTextNickname, "クリック後にニックネームを作成");
                str.Add(StringKey.InitializeButtonGameStart, "入場");
                str.Add(StringKey.MainButtonOpenCreateRoomUI, "作る");
                str.Add(StringKey.MainButtonRoomListRefresh, "リフレッシュ");
                str.Add(StringKey.MainButtonOpenCreateRoomUIInRoomList, "遊ぶ場所を作る");
                str.Add(StringKey.MainButtonCreateRoom, "作る");
                str.Add(StringKey.MainButtonCloseCreateRoomUI, "キャンセル");
                str.Add(StringKey.MainButtonDeveloper, "作成した人々");
                str.Add(StringKey.MainButtonSettings, "設定");
                str.Add(StringKey.MainButtonExitGame, "背景に戻る");
                str.Add(StringKey.MainTextRoomCount, "部屋数");
                str.Add(StringKey.MainTextNoRoom, "作成された部屋はありません");
                str.Add(StringKey.MainTextNoRoomNameInInputField, "部屋の名前を入力してください");
                str.Add(StringKey.MainTextRoomNameTitle, "部屋名");
                str.Add(StringKey.InGameButtonResume, "再開する");
                str.Add(StringKey.InGameButtonSettings, "環境設定");
                str.Add(StringKey.InGameButtonReconnect, "再接続");
                str.Add(StringKey.InGameButtonExit, "出る");
                str.Add(StringKey.ItemHandWarmer, "Hand warmer");
                str.Add(StringKey.ItemUsingHandWarmer, "Hand warmer");
                str.Add(StringKey.ItemColdHandWarmer, "Hand warmer");
                str.Add(StringKey.ItemEmptyBottle, "ボトル");
                str.Add(StringKey.ItemFullBottle, "いっぱいの水瓶");
                str.Add(StringKey.ItemFish, "魚");
                str.Add(StringKey.ItemGrilledFish, "焼き魚");
                str.Add(StringKey.ItemWood, "장작");
                str.Add(StringKey.ItemWoodDescript, "불이 잘 붙을 것 같은 장작이다.");
                str.Add(StringKey.ItemHandWarmerDescript, "사용하면 따뜻해질 것 같은 손난로다.");
                str.Add(StringKey.ItemUsingHandWarmerDescript, "사용해서 따뜻해진 손난로다.");
                str.Add(StringKey.ItemColdHandWarmerDescript, "다 식어버린 손난로다.");
                str.Add(StringKey.ItemEmptyBottleDescript, "물통이 비어있다.");
                str.Add(StringKey.ItemFullBottleDescript, "물이 가득 차있다.");
                str.Add(StringKey.ItemFishDescript, "방금 잡은 듯한 모습을 한 물고기다.");
                str.Add(StringKey.ItemGrilledFishDescript, "바싹 익혀서 노릇노릇 구워진 맛있는 생선이 되었다.");
                str.Add(StringKey.InGameDieMessage, "あなたは{0}に殺された。");
                str.Add(StringKey.InGameMurderPlayer, "みんな殺してください。");
                str.Add(StringKey.InGameNoMurderPlayer, "生き残る必要があります。");
                str.Add(StringKey.InGameDieMessageHungry, "굶주림에 지쳐 사망했습니다");
                str.Add(StringKey.InGameDieMessageFalling, "높은 곳에서 떨어져 사망했습니다");
                str.Add(StringKey.InGameDieMessageVote, "투표로 인해 쫓겨났습니다.");
                str.Add(StringKey.InGameDieMessageNone, "Unknown");
                str.Add(StringKey.InGameMurderWin, "Murderers win");
                str.Add(StringKey.InGamePlayerWin, "Players win");

                str.Add(StringKey.InGameWorkNoMessage, "None");
                str.Add(StringKey.InGameWorkTree, "Tree");
                str.Add(StringKey.InGameWorkWater, "Water");
                str.Add(StringKey.InGameWorkFish, "Fish");
                str.Add(StringKey.InGameWorkOpenVote, "신고하기");

                str.Add(StringKey.InGameWorkProcessTree, "Tree");
                str.Add(StringKey.InGameWorkProcessWater, "Water");
                str.Add(StringKey.InGameWorkProcessFish, "Fish");

                str.Add(StringKey.InGameMessageInventoryIsFull, "아이템을 더 이상 구할 수 없습니다!");
                str.Add(StringKey.InGameMessageNotExistItem, "아이템이 없습니다. \"{0}\"");
                str.Add(StringKey.InGameMessageWorkingTryOtherAction, "작업 중에는 다른 행동을 시도할 수 없습니다.");

                str.Add(StringKey.InGameMessageCompleteVote, "{0}님이 투표를 완료했습니다.");
                str.Add(StringKey.InGameFinishVoteNone, "아무도 내보내지 않았습니다.");
                str.Add(StringKey.InGameFinishVoteMurderTrue, "{0}님은 살인자였습니다.");
                str.Add(StringKey.InGameFinishVoteMurderFalse, "{0}님은 살인자가 아니였습니다.");
                str.Add(StringKey.InGameFinishVoteMurderCount, "살인자가 {0}명 있습니다.");
                break;
            case Language.Chinese:
                str.Add(StringKey.InitializeConnectingMessage, "正在连接服务器...");
                str.Add(StringKey.InitializeTextNickname, "点击并写下您的昵称");
                str.Add(StringKey.InitializeButtonGameStart, "开始");
                str.Add(StringKey.MainButtonOpenCreateRoomUI, "创建");
                str.Add(StringKey.MainButtonRoomListRefresh, "更新");
                str.Add(StringKey.MainButtonOpenCreateRoomUIInRoomList, "创建房间");
                str.Add(StringKey.MainButtonCreateRoom, "创建");
                str.Add(StringKey.MainButtonCloseCreateRoomUI, "消除");
                str.Add(StringKey.MainButtonDeveloper, "制片人");
                str.Add(StringKey.MainButtonSettings, "设置");
                str.Add(StringKey.MainButtonExitGame, "退出游戏");
                str.Add(StringKey.MainTextRoomCount, "房数量");
                str.Add(StringKey.MainTextNoRoom, "没地方玩");
                str.Add(StringKey.MainTextNoRoomNameInInputField, "请输入房间名称");
                str.Add(StringKey.MainTextRoomNameTitle, "房间名称");
                str.Add(StringKey.InGameButtonResume, "撤消");
                str.Add(StringKey.InGameButtonSettings, "环境");
                str.Add(StringKey.InGameButtonReconnect, "重新连接");
                str.Add(StringKey.InGameButtonExit, "出去");
                str.Add(StringKey.ItemHandWarmer, "Hand warmer");
                str.Add(StringKey.ItemUsingHandWarmer, "Hand warmer");
                str.Add(StringKey.ItemColdHandWarmer, "Hand warmer");
                str.Add(StringKey.ItemEmptyBottle, "空的水瓶");
                str.Add(StringKey.ItemFullBottle, "满水瓶");
                str.Add(StringKey.ItemFish, "鱼");
                str.Add(StringKey.ItemGrilledFish, "熟鱼");
                str.Add(StringKey.ItemWood, "장작");
                str.Add(StringKey.ItemWoodDescript, "불이 잘 붙을 것 같은 장작이다.");
                str.Add(StringKey.ItemHandWarmerDescript, "사용하면 따뜻해질 것 같은 손난로다.");
                str.Add(StringKey.ItemUsingHandWarmerDescript, "사용해서 따뜻해진 손난로다.");
                str.Add(StringKey.ItemColdHandWarmerDescript, "다 식어버린 손난로다.");
                str.Add(StringKey.ItemEmptyBottleDescript, "물통이 비어있다.");
                str.Add(StringKey.ItemFullBottleDescript, "물이 가득 차있다.");
                str.Add(StringKey.ItemFishDescript, "방금 잡은 듯한 모습을 한 물고기다.");
                str.Add(StringKey.ItemGrilledFishDescript, "바싹 익혀서 노릇노릇 구워진 맛있는 생선이 되었다.");
                str.Add(StringKey.InGameDieMessage, "你被{0}杀死了。");
                str.Add(StringKey.InGameMurderPlayer, "杀光他们");
                str.Add(StringKey.InGameNoMurderPlayer, "活到最后");
                str.Add(StringKey.InGameDieMessageHungry, "굶주림에 지쳐 사망했습니다");
                str.Add(StringKey.InGameDieMessageFalling, "높은 곳에서 떨어져 사망했습니다");
                str.Add(StringKey.InGameDieMessageVote, "투표로 인해 쫓겨났습니다.");
                str.Add(StringKey.InGameDieMessageNone, "Unknown");
                str.Add(StringKey.InGameMurderWin, "Murderers win");
                str.Add(StringKey.InGamePlayerWin, "Players win");

                str.Add(StringKey.InGameWorkNoMessage, "None");
                str.Add(StringKey.InGameWorkTree, "Tree");
                str.Add(StringKey.InGameWorkWater, "Water");
                str.Add(StringKey.InGameWorkFish, "Fish");
                str.Add(StringKey.InGameWorkOpenVote, "신고하기");

                str.Add(StringKey.InGameWorkProcessTree, "Tree");
                str.Add(StringKey.InGameWorkProcessWater, "Water");
                str.Add(StringKey.InGameWorkProcessFish, "Fish");

                str.Add(StringKey.InGameMessageInventoryIsFull, "아이템을 더 이상 구할 수 없습니다!");
                str.Add(StringKey.InGameMessageNotExistItem, "아이템이 없습니다. \"{0}\"");
                str.Add(StringKey.InGameMessageWorkingTryOtherAction, "작업 중에는 작업을 시도할 수 없습니다.");

                str.Add(StringKey.InGameMessageCompleteVote, "{0}님이 투표를 완료했습니다.");
                str.Add(StringKey.InGameFinishVoteNone, "아무도 내보내지 않았습니다.");
                str.Add(StringKey.InGameFinishVoteMurderTrue, "{0}님은 살인자였습니다.");
                str.Add(StringKey.InGameFinishVoteMurderFalse, "{0}님은 살인자가 아니였습니다.");
                str.Add(StringKey.InGameFinishVoteMurderCount, "살인자가 {0}명 있습니다.");
                break;
        }
    }

    public static string GetString(StringKey key, params object[] args)
    {
        return string.Format(str[key], args);
    }
}
