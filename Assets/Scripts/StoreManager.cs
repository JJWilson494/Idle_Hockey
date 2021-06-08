using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;


/// <summary>
/// Class for managing the in game shop
/// </summary>
public class StoreManager : MonoBehaviour, IStoreListener {

    //Buy 1 hour worth of time. TODO: m_
    [SerializeField]
    private Button buy1HourButton;

    //Buy 6 hour worth of time. TODO: m_
    [SerializeField]
    private Button buy6HourButton;

    //Buy 12 hour worth of time. TODO: m_
    [SerializeField]
    private Button buy12HourButton;

    //Controller for the store
    private static IStoreController m_StoreController;       
    //Extension provider for the store
    private static IExtensionProvider m_StoreExtensionProvider;

    //ID for the 20 trophy purchase
    public static string productID20Trophies = "1";

    //ID for the 50 trophy purchase
    public static string productID50Trophies = "2";

    //ID for the 120 trophy purchase
    public static string productID120Trophies = "3";

    //ID for the 250 trophy purchase
    public static string productID250Trophies = "4";

    //ID for the 550 trophy purchase
    public static string productID550Trophies = "5";

    //ID for the 1500 trophy purchase
    public static string productID1500Trophies = "6";

    //ID for the remove ads purchase
    public static string productIDRemoveAds = "7";

    // Use this for initialization
    void Start ()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    /// <summary>
    /// Determination if we are initialized
    /// </summary>
    /// <returns>Flag for if initialized</returns>
    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    /// <summary>
    /// Attempt to initialize the purchasing information
    /// </summary>
    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        builder.AddProduct(productID20Trophies, ProductType.Consumable);
        builder.AddProduct(productID50Trophies, ProductType.Consumable);
        builder.AddProduct(productID120Trophies, ProductType.Consumable);
        builder.AddProduct(productID250Trophies, ProductType.Consumable);
        builder.AddProduct(productID550Trophies, ProductType.Consumable);
        builder.AddProduct(productID1500Trophies, ProductType.Consumable);
        // Continue adding the non-consumable product.
        builder.AddProduct(productIDRemoveAds, ProductType.NonConsumable);

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>
    /// Buy 1 hour of time
    /// </summary>
    public void BuyOneHour()
    {
        int currency = ProcessGameLogic.GetPremiumCurrency();
        ProcessGameLogic.SetPremiumCurrency(currency - 15);
        BuyTime(3600.0f);
    }

    /// <summary>
    /// Buy 6 hours of time
    /// </summary>
    public void BuySixHours()
    {
        int currency = ProcessGameLogic.GetPremiumCurrency();
        ProcessGameLogic.SetPremiumCurrency(currency - 70);
        BuyTime(21600.0f);
    }

    /// <summary>
    /// Buy 12 hours of time
    /// </summary>
    public void BuyTwelveHours()
    {
        int currency = ProcessGameLogic.GetPremiumCurrency();
        ProcessGameLogic.SetPremiumCurrency(currency - 120);
        BuyTime(43200.0f);
    }

    /// <summary>
    /// Buy 20 trophies
    /// </summary>
    public void Buy20Trophies()
    {
        BuyTrophies("1");
    }

    /// <summary>
    /// Buy 50 trophies
    /// </summary>
    public void Buy50Trophies()
    {
        BuyTrophies("2");
    }

    /// <summary>
    /// Buy 120 trophies
    /// </summary>
    public void Buy120Trophies()
    {
        BuyTrophies("3");
    }

    /// <summary>
    /// Buy 250 trophies
    /// </summary>
    public void Buy250Trophies()
    {
        BuyTrophies("4");
    }

    /// <summary>
    /// Buy 550 trophies
    /// </summary>
    public void Buy550Trophies()
    {
        BuyTrophies("5");
    }

    /// <summary>
    /// Buy 1500 trophies
    /// </summary>
    public void Buy1500Trophies()
    {
        BuyTrophies("6");
    }

    /// <summary>
    /// Buy a certain amount of time
    /// </summary>
    /// <param name="time">The time to purchase</param>
    private void BuyTime(float time)
    {

        NumberWithModifier incomePerSecond = ProcessGameLogic.GetIncomePerSecond();
        incomePerSecond *= time;
        ProcessGameLogic.AddToIncome(incomePerSecond);
    }

    /// <summary>
    /// Purchase trophies
    /// </summary>
    /// <param name="productID">The product ID to purcchase</param>
    private void BuyTrophies( string productID)
    {

        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productID);
            if (product != null)
            {
                Debug.Log("Purchasing trophies");
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("Unable to find that product");
            }
        }
        else
        {
            Debug.Log("Not initialized");
        }
    }

    /// <summary>
    /// Initialization callback
    /// </summary>
    /// <param name="controller">the store controller</param>
    /// <param name="extensions">the extension provider</param>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }

    /// <summary>
    /// Initialization failed callback
    /// </summary>
    /// <param name="error">The associated error</param>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    /// <summary>
    /// Handle purchasing 
    /// </summary>
    /// <param name="args">The purchase event</param>
    /// <returns></returns>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // A consumable product has been purchased by this user.
        if (String.Equals(args.purchasedProduct.definition.id, productID20Trophies, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased
            int currency = ProcessGameLogic.GetPremiumCurrency();
            ProcessGameLogic.SetPremiumCurrency(currency + 20);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, productID50Trophies, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            int currency = ProcessGameLogic.GetPremiumCurrency();
            ProcessGameLogic.SetPremiumCurrency(currency + 50);
        }
        // Or ... a subscription product has been purchased by this user.
        else if (String.Equals(args.purchasedProduct.definition.id, productID120Trophies, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            int currency = ProcessGameLogic.GetPremiumCurrency();
            ProcessGameLogic.SetPremiumCurrency(currency + 120);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, productID250Trophies, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            int currency = ProcessGameLogic.GetPremiumCurrency();
            ProcessGameLogic.SetPremiumCurrency(currency + 250);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, productID550Trophies, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            int currency = ProcessGameLogic.GetPremiumCurrency();
            ProcessGameLogic.SetPremiumCurrency(currency + 550);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, productID1500Trophies, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            int currency = ProcessGameLogic.GetPremiumCurrency();
            ProcessGameLogic.SetPremiumCurrency(currency + 1500);
        }
        // Or ... an unknown product has been purchased by this user. Fill in additional products here....
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// Error provided on purchase failure
    /// </summary>
    /// <param name="product">The attempted product</param>
    /// <param name="failureReason"> The failure reason</param>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}
