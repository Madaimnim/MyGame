using System;

public static class AppearEffectorFactory {
    public static IAppearEffector CreateEffector(AppearType type) {
        switch (type) {
            case AppearType.Instant:
                return new InstantSpawnEffector();

            case AppearType.DropBounce:
                return new DropBounceSpawnEffector();

            default:
                return new InstantSpawnEffector();
        }
    }
}
