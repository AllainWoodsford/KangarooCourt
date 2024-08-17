using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KCStaticEnums 
{
  //ENUM
  public enum SceneNames {
    none,mainMenu,lobby,court
  }

  public enum Verdict{
    none,missTrial,Guilty,NotGuilty
  }

  public enum CourtState{
    none,init,end,newCase,closeCase,
    defendantArg,
    prosecArg,
    witnessDef,
    witnessProsec,
    closingDefArg,
    closingProsecArg,
    jury,
    hearing
  }
  public static string PATH_RESOURCES_SFX = "SFX/";
  public enum SoundNames {
    none,lawAndOrder,click,attention,error,murmur
  }

  public enum PopulateableLists{
    none,people,matters
  }
  //MATTERS~~~~~
  public static int TIMER_DEFENDANT_PLEA = 15;
  public static int TIMER_PROSECUTOR_ARGUMENT = 15;
  public static int TIMER_WITNESS_STATEMENT_DEFENCE = 15;
  public static int TIMER_WITNESS_STATEMENT_PROSECUTION = 15;

  public static int TIMER_DEFENDANT_CLOSING_ARGUMENT = 15;
  public static int TIMER_PROSECUTOR_CLOSING_ARGUMENT = 15;
  public static int COUNTER_MAXIMUM_TRIALS_DEFAULT = 10;
  public static int TIMER_JURY_TIME = 30;
}
