import mathutils


def import_convert_rotation(quaternion, to_mode):
    """
    Convert rotation from QUATERNION to a type of Euler.
    """
    if to_mode in ['XYZ', 'XZY', 'YXZ', 'YZX', 'ZXY', 'ZYX']:
        return mathutils.Quaternion(quaternion).to_euler(to_mode)
    return quaternion

def export_convert_rotation(rotation, from_mode):
    """
    Convert rotation from the given mode to QUATERNION and return as a formatted string.
    """
    if from_mode in ['XYZ', 'XZY', 'YXZ', 'YZX', 'ZXY', 'ZYX']:
        quaternion = mathutils.Euler(rotation, from_mode).to_quaternion()
        #Limiting the decimals to 10, because RenderWare and inherently from that GTA SA doesn't like greater decimals than that...
        f_quaternion = [
            f"{quaternion[1]:.10f}", # X
            f"{quaternion[2]:.10f}", # Y
            f"{quaternion[3]:.10f}", # Z
            f"{quaternion[0]:.10f}"  # W
        ]
        return ", ".join(map(str, f_quaternion))
    return rotation
