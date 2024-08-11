using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KCStaticEnums 
{
  //ENUM
  public enum SceneNames {
    none,mainMenu,lobby,court
  }

  public enum CourtState{
    none,init,end,newCase,closeCase,
    defendantArg,
    prosecArg,
    witnessDef,
    witnessProsec,
    closingDefArg,
    closingProsecArg
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
}
