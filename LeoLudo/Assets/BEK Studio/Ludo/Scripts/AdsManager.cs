using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

namespace BEKStudio{
    public class AdsManager : MonoBehaviour{
        public static AdsManager Instance;
        RewardedAd rewardedAd;
        InterstitialAd interstitialAd;
        BannerView bannerView;


        void Awake() {
            if (Instance == null) {
                Instance = this;
            }
        }

        void Start() {
#if UNITY_ANDROID || UNITY_IPHONE
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            RequestInterstitialAd();
            RequestRewardedAd();
#endif
        }

        void RequestInterstitialAd() {
            if (Application.internetReachability == NetworkReachability.NotReachable) return;

            if (interstitialAd != null){
                interstitialAd.Destroy();
                interstitialAd = null;
            }

            string adUnitId = "";
#if UNITY_ANDROID
            adUnitId = Constants.ADMOB_INTERSTITIAL_ANDROID_ID;
#elif UNITY_IPHONE
            adUnitId = Constants.ADMOB_INTERSTITIAL_IOS_ID;
#endif
            InterstitialAd.Load(adUnitId, new AdRequest(),
                (InterstitialAd ad, LoadAdError error) =>{
                    if (error != null || ad == null){
                        return;
                    }
                    interstitialAd = ad;

                    ad.OnAdFullScreenContentClosed += () =>{
                        RequestInterstitialAd();
                    };

                    ad.OnAdFullScreenContentFailed += (AdError error) =>{
                        RequestInterstitialAd();
                    };
            });
        }

        public void ShowInterstitialAd() {
            if (interstitialAd != null && interstitialAd.CanShowAd()){
                interstitialAd.Show();
            }
        }

        public void RequestBannerAd() {
            if (Application.internetReachability == NetworkReachability.NotReachable) return;

            string adUnitId = "";
#if UNITY_ANDROID
            adUnitId = Constants.ADMOB_BANNER_ANDROID_ID;
#elif UNITY_IPHONE
            adUnitId = Constants.ADMOB_BANNER_IOS_ID;
#endif
            if (bannerView != null){
                bannerView.Destroy();
                bannerView = null;
            }

            bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
            bannerView.LoadAd(new AdRequest());
        }

        public void DestoryBannerAd() {
            if (bannerView != null) {
                bannerView.Destroy();
                bannerView = null;
            }
        }

        void RequestRewardedAd() {
            if (Application.internetReachability == NetworkReachability.NotReachable) return;

            if (rewardedAd != null) {
                rewardedAd.Destroy();
                rewardedAd = null;
            }

            string adUnitId = "";
#if UNITY_ANDROID
            adUnitId = Constants.ADMOB_REWARDED_ANDROID_ID;
#elif UNITY_IPHONE
            adUnitId = Constants.ADMOB_REWARDED_IOS_ID;
#endif

            RewardedAd.Load(adUnitId, new AdRequest(),
                (RewardedAd ad, LoadAdError error) => {
                    if (error != null || ad == null) {
                        return;
                    }

                    rewardedAd = ad;
                });
        }

        //public void ShowRewardedAd() {
        //    if (rewardedAd != null && rewardedAd.CanShowAd()) {
        //        rewardedAd.Show((Reward reward) => {
        //            PlayerPrefs.SetInt("coin", PlayerPrefs.GetInt("coin") + Constants.ADMOB_REWARDED_AD_PRICE);
        //            MenuController.Instance.UpdateCoinText();
        //            RequestRewardedAd();
        //        });
        //    }
        //}


        public void ShowRewardedAd()
        {
#if UNITY_EDITOR

            // fake delay
            StartCoroutine(FakeReward());

#else
            if (rewardedAd == null)
            {
        
                return;
            }

            if (!rewardedAd.CanShowAd())
            {
        
                return;
            }

            rewardedAd.Show((Reward reward) =>
            {
                GiveReward();
                RequestRewardedAd();
            });
#endif
        }

        // For Testing karva mate
        IEnumerator FakeReward()
        {
            yield return new WaitForSeconds(2.5f);
            GiveReward();
        }




        void GiveReward()
        {
            int oldCoin = PlayerPrefs.GetInt("coin");
            int newCoin = oldCoin + Constants.ADMOB_REWARDED_AD_WATCH;

            PlayerPrefs.SetInt("coin", newCoin);
            PlayerPrefs.Save();



            MenuController.Instance.UpdateCoinText();
            //// Profile screen (agar open hai)
            //OnClickProfileButton profile =
            //    FindObjectOfType<OnClickProfileButton>();

            //if (profile != null)
            //    profile.RefreshCoin();

            //MenuController.Instance.profileCoinText.text = newCoin.ToString();  
        }
    }
}