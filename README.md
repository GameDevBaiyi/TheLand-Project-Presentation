# TheLand-Project-Presentation
安卓 IOS 端手游 The Land, 部分代码及功能展示. 包含 Gif 动图, 注意流量.

项目描述: 该项目使用 NGUI. 开发过程比较有趣, 它原本由韩国人开发, 后来被日本人买断, 然后又外包给我们二次换皮开发. 由于该项目的原本版本非常古老, 使用的各种插件和 SDK 升级困难, 文档难寻, 架构不严谨, 开发和维护的成本都极高. 

本人开发或维护的部分功能演示: 
1. 新手引导. 老功能, 由本人维护.
   该项目的新手引导框架并不灵活(留给策划的配置很少), 且不安全(容错处理极其差). 所幸改动的需求也比较少. 

   ![Tutorial1](https://github.com/GameDevBaiyi/TheLand-Project-Presentation/assets/100526832/a2b01fca-3ec7-4ee5-83f8-ce6d84af0bb0)

   ![Tutorial2](https://github.com/GameDevBaiyi/TheLand-Project-Presentation/assets/100526832/3e1d0d9f-af7e-4a31-afda-69b1149ac11f)

   ![Tutorial3](https://github.com/GameDevBaiyi/TheLand-Project-Presentation/assets/100526832/27f1e311-6dea-420e-8cbe-16886a16905b)

2. 大师系统. 新系统, 是该项目为数不多的大型功能, 由本人开发. 
   主要功能是自动工作, 并且大师有不同的 Buff, 对不同的作物有各种属性加成. 当然数据计算主要是 后端服务器 的工作, 我这边处理大师的表现, 寻路 等功能. 

   ![Master1](https://github.com/GameDevBaiyi/TheLand-Project-Presentation/assets/100526832/6fea653d-d638-4ac5-8fb2-60d5c68fa631)

   ![Master2](https://github.com/GameDevBaiyi/TheLand-Project-Presentation/assets/100526832/5e69484f-33d0-47b3-80c7-dc6d54bbc6ef)

3. 精灵系统. 新系统. 
   一个简单的挂件, 和大师共用一套 Buff 系统. 相对于大师系统要简单很多. 

   ![Elf1](https://github.com/GameDevBaiyi/TheLand-Project-Presentation/assets/100526832/46b5fdb0-f0f3-4875-8abd-fa015d442b03)

4. 各种 SDK 接入, 如 IronSource, TalkingData, Adjust 等等. 

5. 游戏上线后, 还实现了各种充值活动, 排行榜等等. 这些功能比较简单, 也是标准的手游前端工作内容, 太多就不过多展示了. 
