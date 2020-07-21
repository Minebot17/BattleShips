using System.Collections.Generic;

public class UsableModuleInfo {
    public List<int> sameModulesIndex;
    public bool isCoolDown;

    public UsableModuleInfo(List<int> sameModulesIndex, bool isCoolDown) {
        this.sameModulesIndex = sameModulesIndex;
        this.isCoolDown = isCoolDown;
    }
}