using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace BricksStyle.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : BricksStyle.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get {return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

       

        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            var group1 = new SampleDataGroup("Group-1",
                 "Getting Started with Debt Cure",
                 "Getting Started with Debt Cure",
                 "Assets/10.jpg",
                 "When you started your debts, you were young, you had nowhere else to go and you basically just needed money to survive back then. Now that you are older, the ghosts of those debts still haunt you, unpaid student loans, payday loans, credit card bills and on top of that you now have a mortgage to pay and your car loan as well.");

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item1",
                 "Starting Out",
                 "Starting Out",
                 "Assets/11.jpg",
                 "If you feel like you have debt problems, do not feel that you are alone. In fact, you may just be an average person, trying to make ends meet, struggling to get through your awful financial crisis, where your pay is not enough to feed the family, keep the house and the car. Almost everyone have debts up to their armpits now and it is not a fantastic state.",
                 "If you feel like you have debt problems, do not feel that you are alone. In fact, you may just be an average person, trying to make ends meet, struggling to get through your awful financial crisis, where your pay is not enough to feed the family, keep the house and the car. Almost everyone have debts up to their armpits now and it is not a fantastic state.\n\nBelow are some tips and simple guidelines that you can follow in starting to deal with your debt issues. These are debt help tips that may or may not work for you, depending on your actual sincerity and commitment to make your financial issues go away. \n\nOn the other hand, if you do not do anything about your debt issues, they surely will not go away so better start something about it, right?\n\nTo jumpstart your way to a better financial position – being debtless, you have to go through this initial step. You have to consolidate all of your debts into one neat file. We are not talking about debt consolidation here, we are pertaining to you making a list of all your loans, your financing schemes that you joined, your credit card bills and any outstanding debt that you have basically. Just write them down on a neat file. \n\nFor each debt that you have, place the following details:\n•	The type of loan\n•	The amount of loan\n•	The name of the financer and contact details\n•	Any reference number or debt number that you have•	The location where you place the statements for this loan•\n	And indicate where you placed a copy of the loan agreement sheet that you signed\n\nOnce you have all these, it is also a good idea to buy a manila folder or basically any type of envelope where you can file all important documentation about your loans. Consolidate these details so you have one place to look for when the step comes for you to actually do something to manage your debt crisis.\n\nAfter you finish your initial list of debts, the next thing you would have to do is to check the different types of debts that you have and sort them the top debts being your urgent priorities and the latter ones being the important but not urgent priorities. \n\nThe urgent debts are the ones that you have to fix and pay attention to first like your mortgage payments or rend overdue fees as these debts may leave you on the streets if you don’t fix them. Also, consider the basic necessities debts like gas, water and electricity. If you do not pay these, you’d end up in a load of inconvenience and trouble.\n\nYour important but not urgent debts are the ones that are very important but may not require your immediate attention like money borrowed from family, health benefits coverage underpayments (this is very important but not urgent) or even meter tickets from the county.\n\nGoing through this would give you some semblance of structure in dealing with your debt difficulties. Start with this and you may be able to work your way to debt freedom.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item2",
                 "Debt Resolution – Follow Through",
                 "Debt Resolution – Follow Through",
                 "Assets/12.jpg",
                 "When you started your debts, you were young, you had nowhere else to go and you basically just needed money to survive back then. Now that you are older, the ghosts of those debts still haunt you, unpaid student loans, payday loans, credit card bills and on top of that you now have a mortgage to pay and your car loan as well.",
                 "When you started your debts, you were young, you had nowhere else to go and you basically just needed money to survive back then. Now that you are older, the ghosts of those debts still haunt you, unpaid student loans, payday loans, credit card bills and on top of that you now have a mortgage to pay and your car loan as well.\n\nSo you decided to take the bull by the horns and do something about your debt problems you have started to take charge by listing all your debt issues and all your outstanding loans into one ledger that you have, compiling all signed agreements for loans and all statements that you have to ensure that all the data you need to meet this issue head on is right at your fingertips. \n\nThe next step that you have to face in debt resolution is that you have to take into consideration the liquid money that you have. We are not talking about tapping into your retirement plan, not talking about selling your company stocks. This is basically the budget that you have based on the salary you get every two weeks, benefits you receive from your ex-spouse and maybe monies coming from other portals of income like if you have an internet business of some sort. \n\nList all your income in a ledger next to your countdown of loans. Then think of other ways you can increase this income that you can still fit in your schedule. Maybe be more active in that affiliate business of yours. Maybe you can sell the apples that you have on your backyard and make an apple stand, it is up to you. Place these in the “future state” income and allot some space for this in your ledger\n\nThen after squeezing your brain out with the income details, give it a few minutes of rest then list down your expenses. This include your mortgage or rent payments, your basic necessities bills, your household charges, school costs for the kids, travel expenses, health plan premiums, emergency monies, IRA deposits and the like.\n\nAdd the income column up and then add the expenses column up. You may see a difference there that you can set aside for your loan repayments. Sometimes, it happens that you can even save up some money. \n\nHowever, more often than not, the expenses are just equal to the income column. Or worse, they are even above the income total. This is the reason why some people get into the debt trap of having to loan over and over again just to get by each month.\n\nIf this is your situation, it may be time to seek assistance from an advice agency on how you can manage your loan. You may also want to look for debt management programs that can help you manage your outstanding debts all in all.\n\nBut remember, it is a good idea to start this debt resolution as early as possible so you can start saving up for your retirement days, or even start saving up for that vacation you have always wanted.",
                 53,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item3",
                 "PAYING OFF YOUR CREDIT CARD DEBT",
                 "PAYING OFF YOUR CREDIT CARD DEBT",
                 "Assets/13.jpg",
                 "It has been said that in this time and in this culture, people have become so used to buying what they want even before they have earned the money to be able to do so. Where else do we see this mostly happening but in those who have been able to apply for credit cards? This is not an admonition to those who have credit cards but to those who have gotten used to whipping them out and spending the money on credit on a whim. Credit cards are good for emergency but when the debt piles up due to too much unnecessary expenses and there’s no way to pay them back, here are some things that you may help you out.",
                 "First off, start checking your own credit reports. That’s right. Don’t rely on the credit card company to give you an accurate report every time. There is a chance you may be charged for something that you didn’t even buy. You have to watch out for this. This will also ensure that you are protected from any fraudulent use of your card (which happens at times) and may even help you reduce the overall amount you have to pay every deadline.\n\nSecond, if you have more than one credit card, pay off those with high interest first. This will ensure that you pay less in terms of interest and it will take you less time to pay it off as well. The longer you wait, the bigger your payments get because of the interest rate, especially if you have a big balance. Another option that you have is to pay off the smaller debts regardless of whether it is the one with higher or the lower interest rate. The rationale behind this is that it creates motivation that will propel you towards paying off your other debts. If you are considering taking this action, what could be suggested is that you sit down and actually calculate how much you’ll spend later on in terms of payment for the high-interest credit card. It may reach to hundreds of dollars but if it’s not that much, then you may vary your strategy to the payment of the smaller debts.\n\nThird, start to gather your available sources of finance in preparation for the payment of your credit card debt. You’ll want to do this because the sooner you get out of debt, the better. However, don’t forfeit your savings. Make sure you’re still able to set aside some money for when you run into financial difficulties later on while you’re in the middle of paying your debt. The last thing you want is more debt. \n\nFourth, make a budget and stick to it. Cut down to the essentials. Then take note of the extra money that you have. One advantage of making a budget is that you’re able to monitor your expenses better and you’re able to see where your finances actually go. In this way, you start taking control of your finances.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item4",
                 "Credit Counseling Services",
                 "Credit Counseling Services",
                 "Assets/14.jpg",
                 "Debts are very hard to let go of not because anyone is quite fond of them but because that is the way that they are. If one finds that he or she is waist deep in debt, it is not always that case that that particular person made the wrong choices in his life. This may have happens because many unfortunate circumstances coming that person’s way.",
                 "Debts are very hard to let go of not because anyone is quite fond of them but because that is the way that they are. If one finds that he or she is waist deep in debt, it is not always that case that that particular person made the wrong choices in his life. This may have happens because many unfortunate circumstances coming that person’s way.\n\nIf you are way behind on your loan repayments and your net pay minus your daily expenses is not enough to pay even the minimum repayments of your loans, if you have already tried your best to meet your situation head on by making a solid listing of all your debts and income but you just could no longer find a way to reconcile your financial situation, then maybe it is time for you to seek professional help. \n\nSeeking professional help in this case is in the form of a credit counseling agent. He or she can help you stop your creditors’ collection agents from calling you up day and night during the most inappropriate times, your counselor can help you get lower or better interest rates, your credit counselor can also help you consolidate those several loans and bills that you have into one neat loan that you will have no trouble in tracking, and he or she can even assist you in removing those awful late payment charges that you have. \n\nYou may think that with such fine services that a credit counseling agent offers, he or she is bound to charge you high on this assistance. But lo and behold, organizations that offer credit counseling services are more often than not run by non-profit institutions that can help you lout in your most trying of times. There are credit counseling agencies that can help you, just check your local yellow pages, you can also see unions, military basis or even universities as these agencies usually reside inside such institutions.\n\nOnce you do find a reputable credit counseling group, you will be given your own credit counselor. Your credit counselor will be your beacon of light in your murky debt situation. He or she will sit down with you and will ask you the details of your current debt situation. Be sure to bring in statements of billing for your loans, loan agreement contracts, mortgage commitment letters and the like when you first meet up with your counselor so he or she can have a clearer picture on where you stand. Then, your counselor will give you the proper advise on money management, he or she will assist you in drafting a budget that you have to stick to and your counselor will also give you some informational packets that can assist you further in your journey to debt liberation.\n\nFrom thereon, depending on the gravity of your situation, your counselor can basically just set up follow up meetings with you or if you really are in a huge hole of debt, he or she may end up asking you to enroll in a debt management program they are affiliated with.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item5",
                 "Debt Settlement",
                 "Debt Settlement",
                 "Assets/15.jpg",
                 "Then you may want to consider another alternative, you may want to check out debt settlement services. Debt settlement companies or debt arbitration companies are institutions that try their best to negotiate (or haggle if you may), with your creditors, trying to make your creditors say OK with an amount that you can pay to pay your original debt amount.",
                 "While negotiations take place, you stop your repayments to your creditors. Instead, the money that you can give, you give it to your debt settlement company so they accumulate it in a non interest bearing account. All the while, you are waiting for the results of the negotiation.\n\nThe negotiation can end up in two ways. You get to either pay the amount in full with the money accumulated, or you get to pay a huge chunk of it, then you have a smaller repayment left per month. In any case, the creditor will be required by your settlement agency to report this as fully paid to credit agencies so that your credit score will no longer be pulled down.\n\nBut how will you pay this service then if on the onset, you do not have the money to pay even your basic loans? Well, the settlement agency will be paid via commission on how much your debt was lowered. Aside from that, there will also be some administrative fees.\n\nThis may be an alternative you may want to research on or consider prior to filing bankruptcy. As we all know, if you file bankruptcy, you may end up having that tarnishing detail in your credit record for the next decade or so. So you may want to hold your horses and see bankruptcy as your final resort in debt removal.\n\nHowever, this does not mean that you should think of debt settlement services as your perfect saving grace in debt liberation. This venue for debt help also has its risk, like the other debt help possibilities. One risk is that your creditor, even if your debt settlement agent tried his very best, might not even agree to settle. This will result to you having bad credit. \n\nAlso, there are some credit settlement companies that are not very good in working with their clients s o you may want to search the web for testimonials on various credit settlement companies before choosing one you will avail. Better yet, see the BBB website to see if they have a positive score there or not.",
                 79,
                 49,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item6",
                 "Getting out of debt",
                 "Getting out of debt",
                 "Assets/16.jpg",
                 "There really is something very admirable of people who are always thinking ahead and planning for the future. This ensures them that all their hard work now will definitely pay off in the end – and they would not have to be worry about becoming saddled with debt. Of course along with all that planning should also be concrete steps – like getting a stable job, investing in a sound business or saving money in the bank, to ensure that all that planning would not go to waste.",
                 "For a fool-proof plan into getting the future you have always wanted, here are easy-to-follow tips that you really would not have any trouble taking into heart as long as you have already set your mind on becoming debt free and more financially responsible.\n\nTip #1 Pay off all your existing debts: before you are able to start planning for your hopefully, financially stress-free future, you should definitely pay off whatever existing debts first – and make sure that you do not add more to it anymore. If your debts are composed more of credit card debt, why not get it consolidated and have it paid off through deferred monthly payments. This way, you will be able to easily pay for your credit card debt – minus the monthly interest rates. Same goes for other possible loans or debts that you may have, just try to talk to a representative of your bank just to see whatever options you may have. Don’t think of banks as the bad guy, as much as possible these banks would love to have you interested paying off your debts, this is why they consistently try to come up with ways to make the process easier for their clients. \n\nTip #2 Manage your monthly income more responsibly: once you get your paycheck, fight off your brain’s initial reaction of “spend, spend, spend” even for just lunch at that new restaurant. Don’t think of this as a way of depriving yourself or not rewarding yourself for all your hard work, stop being pessimistic and just think of this as a way for you to be able to finally have a financially stress-free life. Try to work out a monthly budget for yourself and do try your best to make it realistic. This way you will be able to better keep track of the things that you are spending each month as well as how much money you are able to save. While you may find this a bit of a challenge at first, soon you will be able to get into the groove of being financially responsible.  \n\nTip #3 Start preparing for the future by saving: now that you have the right attitude towards achieving your goal of being a financially responsible person – free from debt, you can now take all your planning to the next level by crafting a feasible financial plan for your future. Let’s say you want to have a business five years down the line, make sure to write down your plans and how you will be able to achieve it. This is actually a way for you to be able to consistently inspire yourself to always have your priorities on straight. ",
                 53,
                 49,
                 group1));
            
            this.AllGroups.Add(group1);

             var group2 = new SampleDataGroup("Group-2",
                 "Directions",
                 "Directions",
                 "Assets/20.jpg",
                 "Having debts is a slippery slope of doom, and you need to know this as early as possible for you to avoid the temptation of getting more debts than you can handle. Sure, you may be able to get what you want, but in the end, you end up just delaying the loss of money.");

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item1",
                 "Debt Trap",
                 "Debt Trap",
                 "Assets/21.jpg",
                 "Debts, financing, loans, etcetera, these are all very enticing when you first start. Like with credit cards, they start with having your loan at a 0% interest rate for the first year. But once you do get used to having your credit line that way, your tendency is just let lose and keep on charging things to your card without paying the full balance in a month. ",
                 "When you were starting out, fresh out of college, with your future in front of you, you felt like the world was your oyster. Then, you had your first job and it seems like the pay was good. So you finance a home, you take out a car loan and you charge your expenses through your plastic, all the while thinking that all these will be paid in due time because you have this wonderful job where you have a steady pay that you can turn to.\n\nAnd then it happens. Either your company closed down or it laid off some workers to offshore to different countries at cheaper labor,  but in anycase, you are now left with no job and no money to support your debt payments. Or you suddenly found yourself having a family that you have to support financially. You find that your family siphons all your salary from you leaving your debts unpaid, burying you deeper and deeper in debt with all the interests that you have to pay in the future. Or worse, your pay is no longer enough and you find yourself asking for more debt just to support your family.\n\nHaving debts is a slippery slope of doom, and you need to know this as early as possible for you to avoid the temptation of getting more debts than you can handle. Sure, you may be able to get what you want, but in the end, you end up just delaying the loss of money.\n\nDebts, financing, loans, etcetera, these are all very enticing when you first start. Like with credit cards, they start with having your loan at a 0% interest rate for the first year. But once you do get used to having your credit line that way, your tendency is just let lose and keep on charging things to your card without paying the full balance in a month. \n\nThis behavior at the first 12 months of use of a credit card fosters the spending mentality that it also changes or shifts how we spend and charge as a whole, making us more and more prone to debts. This is the same mentality fostered in getting car loans or house loans wherein you prolong your payment period, giving you an option to pay or not to pay. And whenever this option is opened, the most likely choice will be to not pay at all.\n\nWith that being said, aside from ruining your credit rating, having debts really has too much negative impacts involved that it is safe to say that debts are very tricky things to avail. More so, they are very tricky things to start, much less have for more than a few years. \n\nGiven that debts do have their own perks, like having the money to buy items you really need as of the moment but the risks very much outweighs the advantages.\n\nSo do a self reflection on what debts you have. You have to acknowledge the state you are in, deep in debt, as this is the first step in debt help.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item2",
                 "CLIMBING OUT OF THE DEBT PIT",
                 "CLIMBING OUT OF THE DEBT PIT",
                 "Assets/22.jpg",
                 "It is easier to get into debt that to get out of it.  The modern society has come a long way from commodity to commodity trading of barter to cashless and credit-based transactions.  Financial institutions are all too willing to give you credit lines in exchange for service fees and interest on unpaid credit.",
                 "It is easier to get into debt that to get out of it.  The modern society has come a long way from commodity to commodity trading of barter to cashless and credit-based transactions.  Financial institutions are all too willing to give you credit lines in exchange for service fees and interest on unpaid credit.  Retail stores and merchants are even more willing to let you use that credit since they are assured that the financial institutions will pay for your purchases regardless of whether you have the equivalent cash or not.  Commercialism has taken center stage in our lives, creating need when there is none in the first place.  People buy things not anymore because they need them but because they are made accessible to them through credit.\n\nYou can get loans for virtually anything nowadays.  Housing loans, automobile loans, student loans, calamity loans, and so many more.  While some of these are legitimate survival needs, these financing instruments can easily be abused by one who lacks money management skills.  Under the general category of personal loans, most have the tendency to balloon because of service charges, fees, and interest rates.  When you have one too many loan instruments to pay for, all growing at exponential rates, it could be extremely difficult if not impossible to climb out of the seemingly bottomless pit of debt.\n\nHelp with debt can be availed of no matter how deep in debt you are.  The most important thing is for you to acknowledge your indebtedness and start doing something to remedy it.  The most obvious option for anyone who is gravely in debt is to file for bankruptcy.  This is not as simple as it sounds and does not in any way mean that you are free from your debt obligation.  Filing for bankruptcy allows you to start building your financial foundation again from scratch.  In an individual bankruptcy filing, an individual is forced to sell his assets to pay for his debts.  A trustee is appointed to oversee the entire process and to make sure that all creditors are paid from the proceeds of the sale.  This kind of bankruptcy filing is called as “liquidiation” bankruptcy as opposed to “rehabilitation” bankruptcy that is often filed by individuals or corporations in business.\n\nAnother way to get help with debt is to contact debt negotiators to do debt settlement.  Help with debt can be availed of by restructuring existing loans into terms that are more manageable for the debtor.  With restructuring, especially for credit card loans, debts are re-aged.  Penalties and interest rates are recomputed based on new tenors.  By going under a restructuring program, loans do not increase uncontrollably but in a new and lower fixed interest rate making it more manageable for the borrower to pay.\n\nDebt counsellors can also help with debt problems.  Those who are still able to pay for their debt obligations but are already worried about the huge amounts of payable with various loan companies can seek advice through debt counselling.  Counsellors can help those heavy with debt analyze their indebtedness and come up with solutions to help them manage their debt payments including but not limited to curbing unnecessary spending.  There are a lot of ways to help with debt problems.  If you are already burdened with multiple debts, the key to surviving the debt trap is to successfully manage debt and consequently erase it at the soonest possible time.",
                 53,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item3",
                 "ERASING BAD DEBT",
                 "ERASING BAD DEBT",
                 "Assets/23.jpg",
                 "When you get deep into debt for no single identifiable reason, it will not be long before you will be needing help with debt.  Such bad debt can cause a lot of trouble when left to accumulate uncontrollably.  Sooner rather than later, you will wake up with bills that you cannot afford to pay.",
                 "When you get deep into debt for no single identifiable reason, it will not be long before you will be needing help with debt.  Such bad debt can cause a lot of trouble when left to accumulate uncontrollably.  Sooner rather than later, you will wake up with bills that you cannot afford to pay.  Worse, you may even resort to taking out more loans to pay off your existing loans, adding even more problem to your already massive debt.  There are ways to erase bad debt but not without some effort on your part.  \n\nErasing bad debt does not mean skirting the responsibility of paying for your debt.  Erasing bad debt involves paying off loans either through restructuring or consolidation.  Contact a debt consultant to help in coordinating with the credit companies to restructure your loans with terms that are easier on your finances.  Help with debt can be successfully achieved through debt restructuring.  This option usually takes a longer time since the paying period is stretched to accommodate your limited financial resources. \n\nAnother way of getting help with debt is consolidation.  When you consolidate, you combine several loan accounts into a single loan.  This is especially helpful for student loans with different interest rates and different paying periods.  Not all debt can be consolidated.  Often, private student loans can only be consolidated with private loan consolidators while federal student loans can be consolidate both in federal and private loan consolidators.  Credit card debt can also be consolidated.  Choose your debt consolidator wisely by comparing charges, fees, and interest rates.\n\nGetting into debt is not entirely bad.  There are good debts and there are bad debts.  When you take out debts for business purposes or for purposes that are projected to eventually generate more funds in the long run, these debts are considered good debts.  These debts are more likely to pay for themselves as the particular endeavors they were used for rake in earnings.  Other debt that are considered good debt are those that are taken out for assets that appreciate in value.  On a worst case scenario, you can liquidate the asset to pay for the loan with some amount of money left for your account.\n\nLoaning for expenses that can better your chances of earning higher income can also be considered good debt.  This includes education, additional learning modules, and training programs that you can use in your professional and business advancement.  The amount of money that you can generate for yourself will be more than enough to settle your loan repayments.  Extra care, however, should be taken to manage these loans properly and make sure that the loan amount applied for is just the right amount to cover educational expenses.  It is easy to get carried away in taking out loans for additional expenses that may not be necessary in getting a good education.\n\nWhether you have good debt or bad debt, it is important that all debts are managed carefully and wisely.  Erasing bad debt should be top priority while payments for existing debt should be maintained. ",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item4",
                 "EFFECTIVE DEBT RELIEF",
                 "EFFECTIVE DEBT RELIEF",
                 "Assets/24.jpg",
                 "When you are deep in debt, debt relief can be achieved in a number of ways.  It is not impossible to get help with debt.  You are not alone in dealing with the burden of loans you can no longer afford to pay.  With the global economic meltdown, hundreds and thousands of people are left without jobs and consequently without income to pay for their loan accountabilities such as home loans, auto loans, and credit card loans.  There is a solution to having debt problems.",
                 "When you are deep in debt, debt relief can be achieved in a number of ways.  It is not impossible to get help with debt.  You are not alone in dealing with the burden of loans you can no longer afford to pay.  With the global economic meltdown, hundreds and thousands of people are left without jobs and consequently without income to pay for their loan accountabilities such as home loans, auto loans, and credit card loans.  There is a solution to having debt problems.\n\nYou can get help with debt from financial consultants.  They will be in the best position to help you find out how deep you are in debt trouble.  They can be quite expensive so if you are organized to go through the process of laying out all your debt and scrutinizing every detail of each loan, then you can do it yourself.  Just stick to the basics.  Which ones are affordable for you to pay off?  Which credit companies have remedies to payment defaults?  Do not be afraid to ask around.  If a personal financial consultant is too expensive for you, you can check online for help with debt in various forums and sites that offer free advice.\n\nThe most common advice that you will get when asking for help with debt is to consolidate your debts.  Debt consolidation allows you to combine all your loan accounts into one loan instrument.  With this, you can negotiate for lower interest rates and longer tenors.  When you consolidate your loans, you are left with only one loan to repay instead of three or four or five.  There is only one loan payment amount and one loan payment date to remember every month.  This makes it easier to manage and a lot harder to forget.  Missed payments are minimized by consolidating loans.  And because the tenors are normally longer, the loan repayment amounts are smaller and easier on the pocket.\n\nAnother way to help with debt problems is debt settlement.  In debt settlement, you can negotiate to slash off a huge amount from the debt to pay off the balance in one lump sum.  While the amount of the lump sum payment should be available upon agreement of settlement, the total amount to be paid is significantly reduced and recurring payments and interests are eliminated.  This option is good for those who already have some amount of money stashed away and who would like to avoid having to repeatedly pay interest that keeps the debt growing.  With debt settlement, you will most likely be charged retainer fees for the settlement negotiator. \n\nIf your debt is still manageable but bordering on alarming, then you can choose to take things into your own hands before it gets out of hand.  The first thing that you can do is to stop spending what you cannot afford to spend.  If you do not have the cash for it, do not purchase it.  Instead of using your funds for purchases, channel your funds towards loan payments that are more than what you are required to pay.  This way, you will be able to pay off your loans sooner than expected.  Do this for all your loan accountabilities and soon you will have a debt-free life.",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Big-Group-2-Item5",
                 "SUCCESSFULLY MANAGING DEBT",
                 "SUCCESSFULLY MANAGING DEBT",
                 "Assets/25.jpg",
                 "Being heavily in debt can wreak havoc in all aspects of one’s life.  It can ruin professional and personal relationships without you even seeing it creep up on you.  It is important to manage debt at the earliest point to prevent it from spiralling uncontrollably.  If you have accumulated some amount of debt from financial institutions and products that exceed the number of fingers you have on one hand.",
                 "The financial crisis isn't making debt paying any easier.  A lot of people are struggling with the unstable economic changes, skyrocketing gas prices, and even problems with bankruptcy that having debt is becoming a very crucial circumstance.  Looking for debt help isn't easy to look for as well but there are solutions which can help you avoid being cornered by those nasty lenders and creditors.  With these helpful options and by doing them constantly you can expect a debt free status pretty soon.\n\nApplying and acquiring a debt consolidation loan is the first choice available for consumers who have more than two to three debts already.  You basically turn your unsecured loan into a secured one, which is usually in the form of equity, in order to pay off your debts.  This will not only help you save a lot of money but will also grant you the opportunity to finally pay off those debts.  What's more, banks as well as lenders are more than willing to lend you money as debt consolidation loans, turning it from an unsecured to a secured loan.  Interesting enough, you can apply for a debt consolidation loan over the internet to make things even more convenient for you.  Lenders have websites offering their services to people who are in need of these types of loans.  The advantage is on your side as the debtor since the prices of lending companies are put up online so you will have the opportunity to compare and choose the best offers available.  \n\nThe second option in order to help liquidize debt is debt management services.  Here, this is where debt management companies negotiate with you for a lower balance on your credit cards, which you are supposed to pay.  This is in a form of one payment, which you will pay to the debt management company and they will forward the money to the credit card companies.  The only caution you have to take note here though is that the rates for service fees may differ distinctly from one debt management company to another.  Some might even try to scam you into paying thousands of dollars on service, and in the end leave you still indebted even more.  Therefore before applying for a particular service in a particular company, look through your options, compare their prices for the services offered, and choose which one offers you the best price suitable for your budget.  \n\nAnother option that you may consider for future deb problems is consulting a financial counselor who can provide you with quality debt help.  Now, here the same cautions apply.  A lot of counselors are more than willing to provide you with their services, but look out for those who are persuading you to avail of their special help, which involves buying their expensive debt help packages and guides.  Look out for those who are focused on helping and counseling you on your spending habits more.  These are people who can be trusted, who have knowledge and experience in debt help, and who can guide you into living a life debt free. ",
                 79,
                 49,
                 group2));

             group2.Items.Add(new SampleDataItem("Small-Group-2-Item6",
                 "Fix your finances",
                 "Fix your finances",
                 "Assets/26.jpg",
                 "Still swiping that credit card away? Well there is absolutely nothing wrong about that if you are able to pay off your outstanding balance – every month – in full. If you can only manage to pay the minimum amount per month – and you still have the audacity to use your credit card for shopping luxuries, then shame, shame, shame on you.",
                 "In this day and age, you should know better than to keep doing the things that you know for certain will only give you more money trouble in the end. As a first advice – and probably the most obvious one at that, take that credit card of yours and put it far, far away. Meaning do not put it in your wallet as it will just be a constant source of temptation each time you head out – especially to the mall. Just store it in a safe place so that in case the time comes that you really do need it, then you will still know where it is plus it will be out of reach of people who might just take advantage of your credit card just lying around there. \n\nCheck out these practical tips to help you get smart on your finances and finally get out of debt: \nTip #1 Be honest about your finances: to help you straighten out your finances, you should definitely be honest with yourself about how much financially in trouble you really are. Do not delude yourself into thinking that you are not that much in debt, just accept the situation and be mature about it. Instead of feeling devastated about how much work you really need to do in order to attain your financial goals, just keep in mind that you got yourself in this mess in the first place so all you really need to do is just shape up and stay focused.  \nTip#2 Find ways to earn some extra income: if your finances are actually worse than what you imagine it to be then you should start thinking if your current salary will be enough to help you pay off your debts and pay off your monthly needs. If you still have to think long and hard about it then chances are it isn’t enough. So what are you going to do? Firstly, don’t just mope around and take pity on yourself. Get off that couch and start looking for another means to earn extra money or get yourself a higher paying job. If you have a special skill or talent, you can also use it to earn a good living on the side. If you are a good graphic artist, then you can start taking small projects on the side, if you are good at baking, you can start selling baked goods, it really is all about being creative as well as being determined to get out of your financial rut that will truly help you to succeed. \nTip #3 Make it a habit to save: once your finances start to pick up – meaning, you can already pay for your debts bit by bit and you have enough for your monthly needs, whatever extra money you end up having after everything’s been paid for each month, should already go to your own bank account. Try to restrain yourself from making any impulse purchases, instead make it a habit to save and soon you will be able to truly afford the good things in life. ",
                 53,
                 49,
                 group2));
            
            this.AllGroups.Add(group2);
			
           
        }
    }
}
