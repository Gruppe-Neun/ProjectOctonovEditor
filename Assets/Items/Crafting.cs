using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Crafting{

    public enum CraftingStationType {
        NONE,
        Tüfteltisch
    }

    public struct Recipe {
        public Recipe(Item.Type[] ing, int[] ingAmount, CraftingStationType station, Item.Type res, int resAmount) {
            ingredients = ing;
            if (ingAmount == null){
                amount = new int[] { 1, 1, 1, 1 };
            } else {
                amount = ingAmount;
            }
            craftingstation = station;
            result = res;
            resultAmount = resAmount;
        }
        public Item.Type[] ingredients;
        public int[] amount;
        public CraftingStationType craftingstation;
        public Item.Type result;
        public int resultAmount;
    }

    private static Recipe[] recipes = new Recipe[0];

    public static void loadTest() {
        //Recipe test0 = new Recipe(new Item.Type[] { Item.Type.Battery, Item.Type.UNDEF, Item.Type.UNDEF, Item.Type.Flashlight }, null, CraftingStationType.NONE, Item.Type.LaserRed, 10);
        //Recipe test1 = new Recipe(new Item.Type[] { Item.Type.Ironplate, Item.Type.Nut, Item.Type.Battery, Item.Type.UNDEF }, null, CraftingStationType.NONE, Item.Type.Olli_Body, 1);
        //insertRecipe(test0);
        //insertRecipe(test1);

    }

    //load recipies from file
    public static void loadRecipes(string filePath) {
        string[] lines = System.IO.File.ReadAllLines(filePath);
        for(int i = 4; i < lines.Length; i++) {
            string ingsTmp = lines[i].Split(':')[1].Split('=')[0];
            string[] ings = ingsTmp.Split(';')[0].Split(',');
            string ingAmountSTmp = ingsTmp.Split(';')[1];
            string[] ingAmountS = new string[4];
            if (ingAmountSTmp.Equals("/")) {
                ingAmountS = null;
            }
            else {
                ingAmountS = ingAmountSTmp.Split(',');
            }

            string station = lines[i].Split(':')[0];
            string resultTmp = lines[i].Split('=')[1];
            string result = resultTmp.Split('-')[0];
            string amount = resultTmp.Split('-')[1];
            
            CreateRecipeFromStrings(ings, ingAmountS, station, result, amount);
        }
    }

    private static void CreateRecipeFromStrings(string[] ingsS,string[] ingAmountS, string stationS, string resultS, string amountS) {

        Item.Type ing1 = (Item.Type)Enum.Parse(typeof(Item.Type), ingsS[0]); 
        Item.Type ing2 = (Item.Type)Enum.Parse(typeof(Item.Type), ingsS[1]);
        Item.Type ing3 = (Item.Type)Enum.Parse(typeof(Item.Type), ingsS[2]);
        Item.Type ing4 = (Item.Type)Enum.Parse(typeof(Item.Type), ingsS[3]);
        int[] ingAmount;
        if (ingAmountS == null) { ingAmount = null; }
        else {
            ingAmount = new int[4];
            for (int i = 0; i < ingAmountS.Length; i++) {
                ingAmount[i] = 0;
                Int32.TryParse(ingAmountS[i], out ingAmount[i]);
            }
        }
        CraftingStationType station = (CraftingStationType)Enum.Parse(typeof(CraftingStationType), stationS);
        Item.Type result = (Item.Type)Enum.Parse(typeof(Item.Type), resultS);
        int amount = 0;
        Int32.TryParse(amountS, out amount);

        Recipe recipe = new Recipe(new Item.Type[] { ing1, ing2, ing3, ing4}, ingAmount, station, result, amount);
        insertRecipe(recipe);
    }

    //sort ingredients and add recipe
    private static void insertRecipe(Recipe insert) {
        if (insert.ingredients.Length != 4 || insert.amount.Length != 4) {
            Debug.Log("invalid recipe");
            return;
        }
        for (int i = 0; i < insert.ingredients.Length; i++) {
            int pos = i - 1;
            while (pos >= 0) {
                if ((int)insert.ingredients[pos]<= (int)insert.ingredients[i]) {
                    break;
                }
                pos--;
            }
            if (pos + 1 != i) {
                Item.Type tempType = insert.ingredients[i];
                int tempAmount = insert.amount[i];
                for (int p = i; p > pos + 1; p--) {
                    insert.ingredients[p] = insert.ingredients[p - 1];
                    insert.amount[p] = insert.amount[p - 1];
                }
                insert.ingredients[pos + 1] = tempType;
                insert.amount[pos + 1] = tempAmount;
            }

        }
        
        Recipe[] temp = new Recipe[recipes.Length + 1];
        for(int i = 0; i < recipes.Length; i++) {
            temp[i] = recipes[i];
        }
        temp[recipes.Length] = insert;
        recipes = temp;
    }    

    public static ItemBehavior getResult(ItemBehavior[] input, CraftingStationType station) {
        if (input.Length != 4) return null;
        for (int i = 0; i < input.Length; i++) {
            if (input[i] == null) {
                input[i] = Item.createItem(Item.Type.UNDEF, 100, new Vector3(0, 0, 0));
            }
        }
        for (int i = 0; i < input.Length; i++) {
            int pos = i - 1;
            while (pos >= 0) {
                if ((int)input[pos].type <= (int)input[i].type) {
                    break;
                }
                pos--;
            }
            if (pos + 1 != i) {
                ItemBehavior temp = input[i];
                for (int p = i; p > pos + 1; p--) {
                    input[p] = input[p - 1];
                }
                input[pos + 1] = temp;
            }

        }
        int recipe = 0;
        while (recipe < recipes.Length) {
            if (input[0].type == recipes[recipe].ingredients[0] &&
               input[1].type == recipes[recipe].ingredients[1] &&
               input[2].type == recipes[recipe].ingredients[2] &&
               input[3].type == recipes[recipe].ingredients[3] &&
               station == recipes[recipe].craftingstation) {

                if (input[0].amount >= recipes[recipe].amount[0] &&
                   input[1].amount >= recipes[recipe].amount[1] &&
                   input[2].amount >= recipes[recipe].amount[2] &&
                   input[3].amount >= recipes[recipe].amount[3]) {

                    ItemBehavior result = Item.createItem(recipes[recipe].result, recipes[recipe].resultAmount, new Vector3(0, 0, 0));
                    for (int i = 0; i < input.Length; i++) {
                        if (input[i].type == Item.Type.UNDEF) {
                            GameObject.Destroy(input[i].gameObject);
                        }
                    }
                    return result;
                } else {
                    for (int i = 0; i < input.Length; i++) {
                        if (input[i].type == Item.Type.UNDEF) {
                            GameObject.Destroy(input[i].gameObject);
                        }
                    }
                    return null;
                }
            } else {
                recipe++;
            }
        }
        for (int i = 0; i < input.Length; i++) {
            if (input[i].type == Item.Type.UNDEF) {
                GameObject.Destroy(input[i].gameObject);
            }
        }
        return null;
    }

    public static ItemBehavior craft(ItemBehavior[] input,CraftingStationType station) {        
        if (input.Length != 4) return null;
        for(int i = 0;i<input.Length;i++) {
            if (input[i] == null) {
                input[i] = Item.createItem(Item.Type.UNDEF, 100, new Vector3(0, 0, 0));
            }
        }
        for (int i = 0; i < input.Length; i++) {
            int pos = i-1;
            while (pos >= 0) {
                if ((int)input[pos].type <= (int)input[i].type) {
                    break;
                }
                pos--;
            }
            if (pos + 1 != i) {
                ItemBehavior temp = input[i];
                for (int p = i; p > pos+1; p--) {
                    input[p] = input[p - 1];
                }
                input[pos + 1] = temp;
            }
           
        }
        int recipe = 0;
        while (recipe < recipes.Length) {
            if(input[0].type==recipes[recipe].ingredients[0] &&
               input[1].type == recipes[recipe].ingredients[1] &&
               input[2].type == recipes[recipe].ingredients[2] &&
               input[3].type == recipes[recipe].ingredients[3] &&
               station == recipes[recipe].craftingstation) {

                if(input[0].amount >= recipes[recipe].amount[0] &&
                   input[1].amount >= recipes[recipe].amount[1] &&
                   input[2].amount >= recipes[recipe].amount[2] &&
                   input[3].amount >= recipes[recipe].amount[3]) {

                    input[0].amount -= recipes[recipe].amount[0];
                    input[1].amount -= recipes[recipe].amount[1];
                    input[2].amount -= recipes[recipe].amount[2];
                    input[3].amount -= recipes[recipe].amount[3];
                    ItemBehavior result = Item.createItem(recipes[recipe].result, recipes[recipe].resultAmount, new Vector3(0, 0, 0));
                    for(int i = 0; i < input.Length; i++) {
                        if (input[i].type == Item.Type.UNDEF) {
                            GameObject.Destroy(input[i].gameObject);
                        }
                    }
                    return result;
                } else {
                    for (int i = 0; i < input.Length; i++) {
                        if (input[i].type == Item.Type.UNDEF) {
                            GameObject.Destroy(input[i].gameObject);
                        }
                    }
                    return null;
                }
            } else {
                recipe++;
            }
        }
        for (int i = 0; i < input.Length; i++) {
            if (input[i].type == Item.Type.UNDEF) {
                GameObject.Destroy(input[i].gameObject);
            }
        }
        return null;
    }

    public static Recipe[] getAll() {
        return recipes;
    }

    public static Recipe[] getByResult(Item.Type get) {
        int count = 0;
        for (int i = 0; i < recipes.Length; i++) if (recipes[i].result == get) count++;
        Recipe[] ret = new Recipe[count];
        count = 0;
        for(int i = 0; i < recipes.Length; i++) {
            if(recipes[i].result == get) {
                ret[count] = recipes[i];
                count++;
            }
        }
        return ret;
    }

    public static Recipe[] getByIgredient(Item.Type get) {
        int count = 0;
        bool[] wanted = new bool[recipes.Length];
        for (int i = 0; i < recipes.Length; i++) {
            if (recipes[i].ingredients[0] == get || recipes[i].ingredients[1] == get  || recipes[i].ingredients[2] == get || recipes[i].ingredients[3] == get) {
                count++;
                wanted[i] = true;
            } else {
                wanted[i] = false;
            }
        }
        Recipe[] ret = new Recipe[count];
        count = 0;
        for (int i = 0; i < recipes.Length; i++) {
            if (wanted[i]) {
                ret[count] = recipes[i];
                count++;
            }
        }
        return ret;
    }

    public static List<Recipe> getListAll() {
        List<Recipe> ret = new List<Recipe>();
        for (int i = 0; i < recipes.Length; i++) ret.Add(recipes[i]);
        return ret;
    }

    public static List<Recipe> getListByResult(Item.Type get) {
        List<Recipe> ret = new List<Recipe>();
        for (int i = 0; i < recipes.Length; i++) if (recipes[i].result == get) ret.Add(recipes[i]);
        return ret;
    }

    public static List<Recipe> getListByIngedient(Item.Type get) {
        List<Recipe> ret = new List<Recipe>();
        for (int i = 0; i < recipes.Length; i++) if (recipes[i].ingredients[0] == get || recipes[i].ingredients[1] == get || recipes[i].ingredients[2] == get || recipes[i].ingredients[3] == get) ret.Add(recipes[i]);
        return ret;
    }

}
